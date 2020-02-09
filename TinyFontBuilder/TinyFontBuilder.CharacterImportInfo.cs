using System;
using System.Collections;
using System.Diagnostics.Contracts;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using DWrite = Win32.DWrite;
using User32 = Win32.User32;
using Gdi32 = Win32.Gdi32;
using FILETIME = System.Runtime.InteropServices.ComTypes.FILETIME;
using System.Windows.Interop;
using System.Text;

namespace Terka.TinyFonts
{
    partial class TinyFontBuilder
    {
        private class CharacterImportInfo
        {
            private static readonly Version Windows7 = new Version(6, 1);

            private GlyphTypeface _typeface;
            public GlyphTypeface Typeface { get { return _typeface; } }

            private GlyphRun _run;
            private double _emSize;

            private AntialiasingLevel _antialiasLevel;
            private volatile bool _dataInitialized;

            private Transform _transform;
            private Point _transformOrigin;

            private CharacterGlyphPair _mapping;
            public CharacterGlyphPair Mapping { get { return _mapping; } }

            private ushort _emHeight;
            public ushort EmHeight { get { return _emHeight; } }

            private short _emBaseline;
            public short EmBaseline { get { return _emBaseline; } }

            private Int32Thickness _emSideBearing;
            public Int32Thickness EmSideBearing { get { return _emSideBearing; } }

            private Int32Vector _emAdvance;
            public Int32Vector EmAdvance { get { return _emAdvance; } }

            private Int32Rect _inkBox;
            public Int32Rect InkBox { get { EnsureBitmap(); return _inkBox; } }

            private BitArray[] _bitmapData;
            public BitArray[] BitmapData { get { EnsureBitmap(); return _bitmapData; } }

            private BitArray _antialiasData;
            public BitArray AntialiasData { get { EnsureBitmap(); return _antialiasData; } }

            public CharacterImportInfo(TinyFontBuilder builderState, CharacterGlyphPair mapping) : this(new BuilderState(builderState), mapping) { }
            public CharacterImportInfo(BuilderState builderState, CharacterGlyphPair mapping)
            {
                Contract.Requires(builderState != null, "Typeface cannot be null.");
                Contract.Requires(mapping != null, "Mapping cannot be null.");

                _mapping = mapping;
                _antialiasLevel = builderState.AntialiasingLevel;
                _typeface = builderState.GlyphTypeface;
                _emSize = builderState.EmSize;

                if (mapping.Glyph.HasValue)
                {
                    InitializeGlyphRun();
                    InitializeTransform(builderState.GlyphTransform, builderState.GlyphTransformOrigin);
                    InitializeMetrics();
                }
                else
                {
                    _dataInitialized = true;
                }
            }

            private void InitializeGlyphRun()
            {
                Contract.Requires(_typeface != null, "Typeface cannot be null.");
                Contract.Requires(_mapping != null, "Mapping cannot be null.");

                _run = new GlyphRun();

                using (InitializedState.Of(_run))
                {
                    _run.GlyphTypeface = _typeface;
                    _run.FontRenderingEmSize = _emSize;
                    _run.GlyphIndices = new ushort[] { _mapping.Glyph.GetValueOrDefault() };
                    _run.AdvanceWidths = new double[] { 0.0 };
                };

                HelperUnsupported.SetTextFormattingMode(_run, TextFormattingMode.Display);
            }
            private void InitializeTransform(Transform transform, Point origin)
            {
                Contract.Requires(_run != null, "GlyphRun must be initialized first.");
                Contract.Requires(_mapping != null, "Mapping cannot be null.");

                ushort glyph = _mapping.Glyph.GetValueOrDefault();

                if (transform == null || transform.Value.IsIdentity)
                {
                    _transform = Transform.Identity;
                    return;
                }

                _transformOrigin = new Point(origin.X * _typeface.AdvanceWidths[glyph], origin.Y * _typeface.AdvanceHeights[glyph]);

                if (_transformOrigin.X == 0 && _transformOrigin.Y == 0)
                    _transform = transform;

                else
                {
                    TransformGroup t = new TransformGroup();

                    Transform backOrigin = new TranslateTransform(-_transformOrigin.X, -_transformOrigin.Y);
                    backOrigin.Freeze();
                    t.Children.Add(backOrigin);

                    t.Children.Add(transform);

                    Transform forwardOrigin = new TranslateTransform(_transformOrigin.X, _transformOrigin.Y);
                    forwardOrigin.Freeze();
                    t.Children.Add(forwardOrigin);

                    _transform = t;
                }
            }
            private void InitializeMetrics()
            {
                Contract.Requires(_typeface != null, "Typeface cannot be null.");
                Contract.Requires(_mapping != null, "Mapping cannot be null.");
                Contract.Requires(_transform != null, "Transform must be initialized first.");

                ushort glyph = _mapping.Glyph.GetValueOrDefault();

                checked
                {
                    _emHeight = (ushort)Round(_typeface.Height * _emSize);
                    _emBaseline = (short)Round(_typeface.Baseline * _emSize);
                }

                // System.Diagnostics.Debug.WriteLine("{2}: {0} {1}",
                // _typeface.LeftSideBearings[glyph] * _emSize,
                // _typeface.RightSideBearings[glyph] * _emSize, (char)(int)_mapping.Character);

                if (_antialiasLevel == AntialiasingLevel.None)
                    _emSideBearing = new Int32Thickness(
                        Round(_typeface.LeftSideBearings[glyph] * _emSize),
                        Round(_typeface.TopSideBearings[glyph] * _emSize),
                        Round(_typeface.RightSideBearings[glyph] * _emSize),
                        Round(_typeface.BottomSideBearings[glyph] * _emSize)
                    );
                else
                    _emSideBearing = new Int32Thickness(
                        Floor(_typeface.LeftSideBearings[glyph] * _emSize),
                        Floor(_typeface.TopSideBearings[glyph] * _emSize),
                        Floor(_typeface.RightSideBearings[glyph] * _emSize),
                        Floor(_typeface.BottomSideBearings[glyph] * _emSize)
                    );

                Rect advance = new Rect(new Size(_typeface.AdvanceWidths[glyph], _typeface.AdvanceHeights[glyph]));
                Rect advanceTransformed = _transform.TransformBounds(advance);

                _emAdvance = new Int32Vector(
                    Round(advanceTransformed.Width * _emSize),
                    Round(advanceTransformed.Height * _emSize)
                );

                if (!_transform.Value.IsIdentity)
                {
                    Rect boundBox = _run.ComputeInkBoundingBox();
                    Rect advanceBox = boundBox.Add(_emSideBearing);

                    Rect boundBoxTransformed = _transform.TransformBounds(boundBox);
                    Rect advanceBoxTransformed = _transform.TransformBounds(advanceBox);

                    Thickness bearingTransformed = boundBox.Subtract(advanceBox);

                    _emSideBearing = new Int32Thickness(
                        Round(bearingTransformed.Left),
                        Round(bearingTransformed.Top),
                        Round(bearingTransformed.Right),
                        Round(bearingTransformed.Bottom)
                    );
                }
            }

            private static double TransformX(double x, Transform t)
            {
                return (new Vector(x, 0) * t.Value).X;
            }
            private static double TransformY(double y, Transform t)
            {
                return (new Vector(0, y) * t.Value).Y;
            }

            private void EnsureBitmap()
            {
                if (_dataInitialized == false)
                {
                    _dataInitialized = true;

                    WriteableBitmap bitmap = GetWriteableBitmap();

                    if (bitmap == null)
                        return;

                    _inkBox = ToTinyBitmap1bpp(bitmap, _antialiasLevel, out _bitmapData, out _antialiasData);
                }
            }

            private WriteableBitmap GetWriteableBitmap()
            {
                if (_run == null)
                    return null;

                if (Environment.OSVersion.Version >= Windows7)
                    return GetWriteableBitmapDWrite();
                else
                    return GetWriteableBitmapWpf();
            }

            private static bool Target1Failed;
            private WriteableBitmap GetWriteableBitmapDWrite()
            {
                Rect boundBox = _run.ComputeInkBoundingBox();
                Rect alignBox = _run.ComputeAlignmentBox();

                alignBox.X = boundBox.X;
                alignBox.Width = boundBox.Width;

                Int32Rect transformedAlign = Round(_transform.TransformBounds(alignBox));
                TranslateTransform translate = new TranslateTransform(-Round(transformedAlign.X), -Round(alignBox.Y));
                MatrixTransform finalTransform = new MatrixTransform(_transform.Value * translate.Value);


                uint width = (uint)transformedAlign.Width;
                uint height = (uint)transformedAlign.Height;

                DWrite.IDWriteFactory factory = DWrite.CreateFactory(DWrite.FactoryType.Shared);
                DWrite.IDWriteGdiInterop interop = factory.GetGdiInterop();

                DWrite.IDWriteFontFile font = factory.CreateFontFileReference(_typeface.FontUri.LocalPath, IntPtr.Zero);
                
                bool isSupported; DWrite.FontFileType fileType; DWrite.FontFaceType faceType; uint faceCount;
                font.Analyze(out isSupported, out fileType, out faceType, out faceCount);
                DWrite.IDWriteFontFace face = factory.CreateFontFace(faceType, 1, new DWrite.IDWriteFontFile[] { font }, 0, (DWrite.FontSimulations)_typeface.StyleSimulations);

                DWrite.GlyphRun run = new DWrite.GlyphRun(_run.GlyphIndices[0]);
                run.EmSize = (float)_run.FontRenderingEmSize;
                run.FontFace = face;

                DWrite.IDWriteRenderingParams pars = factory.CreateCustomRenderingParams(1, 0, 0, DWrite.PixelGeometry.Flat, _antialiasLevel == AntialiasingLevel.None ? DWrite.RenderingMode.Aliased : DWrite.RenderingMode.GdiClassic);
                DWrite.Matrix matrix = (DWrite.Matrix)finalTransform.Value;

                IntPtr screenDC = User32.GetScreenDC();

                pass:
                DWrite.IDWriteBitmapRenderTarget target = interop.CreateBitmapRenderTarget(screenDC, width, height);
                target.SetPixelsPerDip(1);
                target.SetCurrentTransform(ref matrix);
                if (!Target1Failed)
                    try
                    {
                        DWrite.IDWriteBitmapRenderTarget1 target1 = (DWrite.IDWriteBitmapRenderTarget1)target;
                        target1.SetTextAntialiasMode(DWrite.TextAntialiasMode.GrayScale);
                    }
                    catch (COMException) { Target1Failed = true; }

                Win32.Rect rect = target.DrawGlyphRun(0, 0, DWrite.MeasuringMode.GdiClassic, ref run, pars, 0xFFFFFF);

                if (rect.Right > width || rect.Bottom > height)
                {
                    if (finalTransform.Matrix.Determinant == 1.0 && finalTransform.Matrix.M11 == 1 && finalTransform.Matrix.M12 == 0) // translation
                    {
                        // if translation was requested, we need to preserve absolute dimensions
                        width = (uint)rect.Right;
                        height = (uint)rect.Bottom;
                    }
                    else
                    {
                        // baseline coordinates are subject to transformation, but we need translation
                        width = (uint)(rect.Right - rect.Left);
                        height = (uint)(rect.Bottom - rect.Top);

                        matrix.OffsetX += -rect.Left;
                        matrix.OffsetY += -rect.Top;
                    }

                    // no easy way to measure ahead
                    goto pass;
                }

                User32.ReleaseScreenDC(screenDC);
                IntPtr memoryDC = target.GetMemoryDC();
                IntPtr hBitmap = Gdi32.GetCurrentObject(memoryDC, Gdi32.ObjectType.Bitmap);

                BitmapSource bitmap = Imaging.CreateBitmapSourceFromHBitmap(hBitmap, IntPtr.Zero, System.Windows.Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
                
                if (Target1Failed)
                    return new WriteableBitmap(new FormatConvertedBitmap(bitmap, PixelFormats.Bgr24, null, 0));
                else
                    return new WriteableBitmap(bitmap);

                // DWrite will clean-up the bitmap/memory DC
                // GC will clean-up DWrite
            }
            private WriteableBitmap GetWriteableBitmapWpf()
            {
                Rect boundBox = _run.ComputeInkBoundingBox();
                Rect alignBox = _run.ComputeAlignmentBox();

                alignBox.X = boundBox.X;
                alignBox.Width = boundBox.Width;

                Int32Rect transformedAlign = Round(_transform.TransformBounds(alignBox));
                TranslateTransform translate = new TranslateTransform(-Round(transformedAlign.X), -Round(alignBox.Y));
                MatrixTransform finalTransform = new MatrixTransform(_transform.Value * translate.Value);



                DrawingVisualWithModes glyphVisual = new DrawingVisualWithModes();

                if (_antialiasLevel == AntialiasingLevel.None)
                {
                    glyphVisual.VisualTextRenderingMode = TextRenderingMode.Aliased;
                    glyphVisual.VisualEdgeMode = EdgeMode.Aliased;
                }
                else
                {
                    glyphVisual.VisualTextRenderingMode = TextRenderingMode.Grayscale;
                    glyphVisual.VisualEdgeMode = EdgeMode.Unspecified;
                }

                using (DrawingContext context = glyphVisual.RenderOpen())
                {
                    context.PushTransform(finalTransform);
                    context.DrawGlyphRun(Brushes.White, _run);
                }

                int width = Round(Math.Max(glyphVisual.ContentBounds.Right, transformedAlign.Width));
                int height = Round(Math.Max(glyphVisual.ContentBounds.Bottom, transformedAlign.Height));

                // Possibility of hitting the limit of GDI objects per process here, seems the finalizers do not have opportunity to run
                // c.f. http://social.msdn.microsoft.com/Forums/en-US/02638996-e45c-4367-8d77-2157afd0241c/rendertargetbitmap-milerrwin32error

                RenderTargetBitmap bitmap;

                bool secondAttempt = false;
                attempt:
                try
                {
                    bitmap = new RenderTargetBitmap(width, height, 96, 96, PixelFormats.Pbgra32);
                }
                catch (COMException)
                {
                    if (secondAttempt)
                        throw;

                    GC.WaitForPendingFinalizers();
                    secondAttempt = true;
                    goto attempt;
                }

                bitmap.Render(glyphVisual);
                bitmap.Freeze();

                return new WriteableBitmap(bitmap);
            }

            private static Int32Rect ToTinyBitmap1bpp(WriteableBitmap glyphBitmap, AntialiasingLevel antialiasLevel, out BitArray[] bitmapDataRows, out BitArray antialiasData)
            {
                Contract.Requires(glyphBitmap != null, "Glyph bitmap cannot be null.");
                Contract.Requires(glyphBitmap.Format.BitsPerPixel == 32, "Glyph bitmap must be in 32 bits per pixel format.");

                int height = glyphBitmap.PixelHeight;
                int width = glyphBitmap.PixelWidth;

                Int32Thickness inkBox = new Int32Thickness(width, height, -1, -1);

                bitmapDataRows = new BitArray[height];
                antialiasData = null;

                int antialiasStep = 0;
                if (antialiasLevel != AntialiasingLevel.None)
                {
                    antialiasStep = OpacityOpaque / ((int)antialiasLevel * (int)antialiasLevel);
                    antialiasData = new BitArray(0);
                }

                for (int y = 0; y < bitmapDataRows.Length; y++)
                {
                    BitArray bitmapDataRow = bitmapDataRows[y] = new BitArray(width);

                    for (int x = 0; x < width; x++)
                    {
                        byte pixel = Marshal.ReadByte(glyphBitmap.BackBuffer, y * glyphBitmap.BackBufferStride + x * glyphBitmap.Format.BitsPerPixel / BitsPerByte + 0);

                        if (pixel > 0)
                        {
                            bitmapDataRow[x] = true;

                            if (y < inkBox.Top) inkBox.Top = y;
                            if (x < inkBox.Left) inkBox.Left = x;

                            if (y > inkBox.Bottom) inkBox.Bottom = y;
                            if (x > inkBox.Right) inkBox.Right = x;

                            if (antialiasLevel != AntialiasingLevel.None)
                                antialiasData.ApendMsb(pixel / antialiasStep, (int)antialiasLevel);
                        }
                    }
                }

                int boxWidth = Math.Max(0, inkBox.Right - inkBox.Left + 1);
                int boxHeight = Math.Max(0, inkBox.Bottom - inkBox.Top + 1);
                return new Int32Rect(inkBox.Left, inkBox.Top, boxWidth, boxHeight);
            }

            private static int Round(double d)
            {
                return (int)Math.Round(d, MidpointRounding.AwayFromZero);
            }
            private static int Floor(double d)
            {
                if (d >= 0)
                    return (int)Math.Floor(d);
                else
                    return (int)Math.Floor(d) + 1;
            }
            private static int Ceiling(double d)
            {
                if (d >= 0)
                    return (int)Math.Ceiling(d);
                else
                    return (int)Math.Ceiling(d) - 1;
            }

            private static Int32Rect Round(Rect r)
            {
                return new Int32Rect(Round(r.X), Round(r.Y), Round(r.Width), Round(r.Height));
            }
            private static Int32Rect Floor(Rect r)
            {
                return new Int32Rect(Floor(r.X), Floor(r.Y), Floor(r.Width), Floor(r.Height));
            }
            private static Int32Rect Ceiling(Rect r)
            {
                return new Int32Rect(Ceiling(r.X), Ceiling(r.Y), Ceiling(r.Width), Ceiling(r.Height));
            }

            [System.Diagnostics.Conditional("DEBUG")]
            public void Export(string path)
            {
                PngBitmapEncoder png = new PngBitmapEncoder();
                png.Frames.Add(BitmapFrame.Create(GetWriteableBitmap()));

                using (System.IO.Stream file = System.IO.File.Create(path))
                    png.Save(file);
            }
        }
    }
}