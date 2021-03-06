﻿using Sheet.Block;
using Sheet.Entry;
using Sheet.Item;
using Sheet.Plugins;
using Sheet.UI;
using Sheet.Util;
using Sheet.WPF;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Sheet.Controller
{
    public class InputArgs
    {
        // Mouse Generic
        public bool OnlyControl { get; set; }
        public bool OnlyShift { get; set; }
        public ItemType SourceType { get; set; }
        public ImmutablePoint SheetPosition { get; set; }
        public ImmutablePoint RootPosition { get; set; }
        public Action<bool> Handled { get; set; }
        // Mouse Wheel
        public int Delta { get; set; }
        // Mouse Down
        public InputButton Button { get; set; }
        public int Clicks { get; set; }
    }

    public enum InputButton
    {
        None,
        Left,
        Right,
        Middle
    }

    public enum SheetCursor
    {
        Unknown,
        Normal,
        Move,
        Pan
    }

    public enum SheetMode
    {
        None,
        Selection,
        Insert,
        Pan,
        Move,
        Edit,
        Point,
        Line,
        Rectangle,
        Ellipse,
        Text,
        Image,
        TextEditor
    }

    public class SheetOptions
    {
        public double PageOriginX { get; set; }
        public double PageOriginY { get; set; }
        public double PageWidth { get; set; }
        public double PageHeight { get; set; }
        public double SnapSize { get; set; }
        public double GridSize { get; set; }
        public double FrameThickness { get; set; }
        public double GridThickness { get; set; }
        public double SelectionThickness { get; set; }
        public double LineThickness { get; set; }
        public double HitTestSize { get; set; }
        public int DefaultZoomIndex { get; set; }
        public int MaxZoomIndex { get; set; }
        public double[] ZoomFactors { get; set; }
        public double Zoom { get; set; }
        public double PanX { get; set; }
        public double PanY { get; set; }
    }

    public class SheetState
    {
        public IHistoryController HistoryController { get; set; }
        public ILibraryController LibraryController { get; set; }
        public IZoomController ZoomController { get; set; }
        public ICursorController CursorController { get; set; }

        public SheetOptions Options { get; set; }

        public ISheet EditorSheet { get; set; }
        public ISheet BackSheet { get; set; }
        public ISheet ContentSheet { get; set; }
        public ISheet OverlaySheet { get; set; }

        public ISheetView View { get; set; }

        public double LastFinalWidth { get; set; }
        public double LastFinalHeight { get; set; }

        public SheetMode Mode { get; set; }
        public SheetMode TempMode { get; set; }

        public IBlock SelectedBlock { get; set; }
        public IBlock ContentBlock { get; set; }
        public IBlock FrameBlock { get; set; }
        public IBlock GridBlock { get; set; }

        public IRectangle TempSelectionRect { get; set; }
        public bool IsFirstMove { get; set; }
        public ImmutablePoint PanStartPoint { get; set; }
        public ImmutablePoint SelectionStartPoint { get; set; }
        public ItemType SelectedType { get; set; }
        public ILine SelectedLine { get; set; }
        public IThumb LineThumbStart { get; set; }
        public IThumb LineThumbEnd { get; set; }
        public IElement SelectedElement { get; set; }
        public IThumb ThumbTopLeft { get; set; }
        public IThumb ThumbTopRight { get; set; }
        public IThumb ThumbBottomLeft { get; set; }
        public IThumb ThumbBottomRight { get; set; }
    }

    public interface ISheet
    {
        double Width { get; set; }
        double Height { get; set; }
        bool IsCaptured { get; }
        object GetParent();
        void SetParent(object parent);
        void Add(IElement element);
        void Remove(IElement element);
        void Add(object element);
        void Remove(object element);
        void Capture();
        void ReleaseCapture();
    }

    public interface ISheetController
    {
        // Properties
        SheetState State { get; set; }

        // Init
        void Init();

        // Clipboard
        void CutText();
        void CopyText();
        void PasteText();
        void CutJson();
        void CopyJson();
        void PasteJson();

        // Delete
        void Delete(IBlock block);
        void Delete();

        // Select All
        void SelecteAll();

        // Deselect All
        void DeselectAll();

        // Toggle Fill
        void ToggleFill();

        // Insert Mode
        void CreateBlock();
        void BreakBlock();

        // Move Mode
        void MoveUp();
        void MoveDown();
        void MoveLeft();
        void MoveRight();

        // Pan & Zoom Mode
        void SetAutoFitSize(double finalWidth, double finalHeight);
        void AdjustBackThickness(double zoom);
        void AdjustPageThickness(double zoom);

        // Data Binding
        bool BindDataToBlock(ImmutablePoint p, DataItem dataItem);
        bool BindDataToBlock(IBlock block, DataItem dataItem);
        void TryToBindData(ImmutablePoint p, DataItem dataItem);

        // New Page
        void NewPage();

        // Open Page
        Task OpenTextPage(string path);
        Task OpenJsonPage(string path);
        void OpenPage();

        // Save Page
        void SaveTextPage(string path);
        void SaveJsonPage(string path);
        void SavePage();

        // Export Page
        void Export(IEnumerable<BlockItem> blocks);
        void Export(SolutionEntry solution);
        void ExportPage();

        // Library
        void Insert(ImmutablePoint p);
        IBlock Insert(BlockItem blockItem, ImmutablePoint p, bool select);
        Task LoadLibrary(string path);
        void LoadLibrary();

        // Input
        void LeftDown(InputArgs args);
        void LeftUp(InputArgs args);
        void Move(InputArgs args);
        void RightDown(InputArgs args);
        void RightUp(InputArgs args);
        void Wheel(int delta, ImmutablePoint position);
        void Down(InputArgs args);

        // Page
        void SetPage(string text);
        string GetPage();
        void ExportPage(string text);
        void ExportPages(IEnumerable<string> texts);
        BlockItem SerializePage();
        void DeserializePage(BlockItem page);
        void ResetPage();
        void ResetPageContent();

        // Plugins
        void InvertSelectedLineStart();
        void InvertSelectedLineEnd();
    }

    public interface IZoomController
    {
        int ZoomIndex { get; set; }
        double Zoom { get; set; }
        double PanX { get; set; }
        double PanY { get; set; }
        void AutoFit();
        void ActualSize();
    }

    public interface IHistoryController
    {
        void Register(string message);
        void Reset();
        void Undo();
        void Redo();
    }

    public interface ITextController
    {
        void Set(Action<string> ok, Action cancel, string title, string label, string text);
    }

    public interface ILibraryController
    {
        BlockItem GetSelected();
        void SetSelected(BlockItem block);
        IEnumerable<BlockItem> GetSource();
        void SetSource(IEnumerable<BlockItem> source);
    }

    public interface ICursorController
    {
        void Set(SheetCursor cursor);
        SheetCursor Get();
    }

    public interface IPageFactory
    {
        void CreateLine(ISheet sheet, IList<ILine> lines, double thickness, double x1, double y1, double x2, double y2, ItemColor stroke);
        void CreateText(ISheet sheet, IList<IText> texts, string content, double x, double y, double width, double height, int halign, int valign, double size, ItemColor foreground);
        void CreateFrame(ISheet sheet, IBlock block, double size, double thickness, ItemColor stroke);
        void CreateGrid(ISheet sheet, IBlock block, double startX, double startY, double width, double height, double size, double thickness, ItemColor stroke);

        IRectangle CreateSelectionRectangle(double thickness, double x, double y, double width, double height);
    }

    public class SheetLineMode
    {
        #region IoC

        private readonly IServiceLocator _serviceLocator;
        private readonly IBlockController _blockController;
        private readonly IBlockFactory _blockFactory;
        private readonly IBlockHelper _blockHelper;
        private readonly IItemController _itemController;
        private readonly IPointController _pointController;

        private readonly SheetState _state;

        public SheetLineMode(IServiceLocator serviceLocator, SheetState state)
        {
            this._serviceLocator = serviceLocator;
            this._blockController = serviceLocator.GetInstance<IBlockController>();
            this._blockFactory = serviceLocator.GetInstance<IBlockFactory>();
            this._blockHelper = serviceLocator.GetInstance<IBlockHelper>();
            this._itemController = serviceLocator.GetInstance<IItemController>();
            this._pointController = serviceLocator.GetInstance<IPointController>();

            this._state = state;
        }

        #endregion

        #region Fields

        private ILine TempLine;
        private IEllipse TempStartEllipse;
        private IEllipse TempEndEllipse;

        #endregion

        #region Methods

        public void Init(ImmutablePoint p, IPoint start)
        {
            double x = _itemController.Snap(p.X, _state.Options.SnapSize);
            double y = _itemController.Snap(p.Y, _state.Options.SnapSize);

            TempLine = _blockFactory.CreateLine(_state.Options.LineThickness / _state.ZoomController.Zoom, x, y, x, y, ItemColors.Black);

            if (start != null)
            {
                TempLine.Start = start;
            }

            TempStartEllipse = _blockFactory.CreateEllipse(_state.Options.LineThickness / _state.ZoomController.Zoom, x - 4.0, y - 4.0, 8.0, 8.0, true, ItemColors.Black, ItemColors.Black);
            TempEndEllipse = _blockFactory.CreateEllipse(_state.Options.LineThickness / _state.ZoomController.Zoom, x - 4.0, y - 4.0, 8.0, 8.0, true, ItemColors.Black, ItemColors.Black);

            _state.OverlaySheet.Add(TempLine);
            _state.OverlaySheet.Add(TempStartEllipse);
            _state.OverlaySheet.Add(TempEndEllipse);
            _state.OverlaySheet.Capture();
        }

        public void Move(ImmutablePoint p)
        {
            double x = _itemController.Snap(p.X, _state.Options.SnapSize);
            double y = _itemController.Snap(p.Y, _state.Options.SnapSize);
            double x2 = _blockHelper.GetX2(TempLine);
            double y2 = _blockHelper.GetY2(TempLine);
            if (Math.Round(x, 1) != Math.Round(x2, 1)
                || Math.Round(y, 1) != Math.Round(y2, 1))
            {
                _blockHelper.SetX2(TempLine, x);
                _blockHelper.SetY2(TempLine, y);
                _blockHelper.SetLeft(TempEndEllipse, x - 4.0);
                _blockHelper.SetTop(TempEndEllipse, y - 4.0);
            }
        }

        public void Finish(IPoint end)
        {
            double x1 = _blockHelper.GetX1(TempLine);
            double y1 = _blockHelper.GetY1(TempLine);
            double x2 = _blockHelper.GetX2(TempLine);
            double y2 = _blockHelper.GetY2(TempLine);

            if (Math.Round(x1, 1) == Math.Round(x2, 1) && Math.Round(y1, 1) == Math.Round(y2, 1))
            {
                Cancel();
            }
            else
            {
                if (end != null)
                {
                    TempLine.End = end;
                }

                _state.OverlaySheet.ReleaseCapture();
                _state.OverlaySheet.Remove(TempLine);
                _state.OverlaySheet.Remove(TempStartEllipse);
                _state.OverlaySheet.Remove(TempEndEllipse);

                _state.HistoryController.Register("Create Line");

                if (TempLine.Start != null)
                {
                    _pointController.ConnectStart(TempLine.Start, TempLine);
                }

                if (TempLine.End != null)
                {
                    _pointController.ConnectEnd(TempLine.End, TempLine);
                }

                _state.ContentBlock.Lines.Add(TempLine);
                _state.ContentSheet.Add(TempLine);

                TempLine = null;
                TempStartEllipse = null;
                TempEndEllipse = null;
            }
        }

        public void Cancel()
        {
            _state.OverlaySheet.ReleaseCapture();
            _state.OverlaySheet.Remove(TempLine);
            _state.OverlaySheet.Remove(TempStartEllipse);
            _state.OverlaySheet.Remove(TempEndEllipse);
            TempLine = null;
            TempStartEllipse = null;
            TempEndEllipse = null;
        }

        public void Reset()
        {
            if (TempLine != null)
            {
                _state.OverlaySheet.Remove(TempLine);
                TempLine = null;
            }

            if (TempStartEllipse != null)
            {
                _state.OverlaySheet.Remove(TempStartEllipse);
                TempLine = null;
            }

            if (TempEndEllipse != null)
            {
                _state.OverlaySheet.Remove(TempEndEllipse);
                TempEndEllipse = null;
            }
        }

        public void Adjust(double zoom)
        {
            double lineThicknessZoomed = _state.Options.LineThickness / zoom;

            if (TempLine != null)
            {
                _blockHelper.SetStrokeThickness(TempLine, lineThicknessZoomed);
            }

            if (TempStartEllipse != null)
            {
                _blockHelper.SetStrokeThickness(TempStartEllipse, lineThicknessZoomed);
            }

            if (TempEndEllipse != null)
            {
                _blockHelper.SetStrokeThickness(TempEndEllipse, lineThicknessZoomed);
            }
        }

        public void ToggleFill()
        {
        }

        #endregion
    }

    public class SheetRectangleMode
    {
        #region IoC

        private readonly IServiceLocator _serviceLocator;
        private readonly IBlockController _blockController;
        private readonly IBlockFactory _blockFactory;
        private readonly IBlockHelper _blockHelper;
        private readonly IItemController _itemController;
        private readonly IPointController _pointController;

        private readonly SheetState _state;

        public SheetRectangleMode(IServiceLocator serviceLocator, SheetState state)
        {
            this._serviceLocator = serviceLocator;
            this._blockController = serviceLocator.GetInstance<IBlockController>();
            this._blockFactory = serviceLocator.GetInstance<IBlockFactory>();
            this._blockHelper = serviceLocator.GetInstance<IBlockHelper>();
            this._itemController = serviceLocator.GetInstance<IItemController>();
            this._pointController = serviceLocator.GetInstance<IPointController>();

            this._state = state;
        }

        #endregion

        #region Fields

        private IRectangle TempRectangle;
        private ImmutablePoint SelectionStartPoint;

        #endregion

        #region Methods

        public void Init(ImmutablePoint p)
        {
            double x = _itemController.Snap(p.X, _state.Options.SnapSize);
            double y = _itemController.Snap(p.Y, _state.Options.SnapSize);
            SelectionStartPoint = new ImmutablePoint(x, y);
            TempRectangle = _blockFactory.CreateRectangle(_state.Options.LineThickness / _state.ZoomController.Zoom, x, y, 0.0, 0.0, false, ItemColors.Black, ItemColors.Transparent);
            _state.OverlaySheet.Add(TempRectangle);
            _state.OverlaySheet.Capture();
        }

        public void Move(ImmutablePoint p)
        {
            double sx = SelectionStartPoint.X;
            double sy = SelectionStartPoint.Y;
            double x = _itemController.Snap(p.X, _state.Options.SnapSize);
            double y = _itemController.Snap(p.Y, _state.Options.SnapSize);
            _blockHelper.SetLeft(TempRectangle, Math.Min(sx, x));
            _blockHelper.SetTop(TempRectangle, Math.Min(sy, y));
            _blockHelper.SetWidth(TempRectangle, Math.Abs(sx - x));
            _blockHelper.SetHeight(TempRectangle, Math.Abs(sy - y));
        }

        public void Finish()
        {
            double x = _blockHelper.GetLeft(TempRectangle);
            double y = _blockHelper.GetTop(TempRectangle);
            double width = _blockHelper.GetWidth(TempRectangle);
            double height = _blockHelper.GetHeight(TempRectangle);
            if (width == 0.0 || height == 0.0)
            {
                Cancel();
            }
            else
            {
                _state.OverlaySheet.ReleaseCapture();
                _state.OverlaySheet.Remove(TempRectangle);
                _state.HistoryController.Register("Create Rectangle");
                _state.ContentBlock.Rectangles.Add(TempRectangle);
                _state.ContentSheet.Add(TempRectangle);
                TempRectangle = null;
            }
        }

        public void Cancel()
        {
            _state.OverlaySheet.ReleaseCapture();
            _state.OverlaySheet.Remove(TempRectangle);
            TempRectangle = null;
        }

        public void Reset()
        {
            if (TempRectangle != null)
            {
                _state.OverlaySheet.Remove(TempRectangle);
                TempRectangle = null;
            }
        }

        public void Adjust(double zoom)
        {
            double lineThicknessZoomed = _state.Options.LineThickness / zoom;

            if (TempRectangle != null)
            {
                _blockHelper.SetStrokeThickness(TempRectangle, lineThicknessZoomed);
            }
        }

        public void ToggleFill()
        {
            if (TempRectangle != null)
            {
                _blockController.ToggleFill(TempRectangle);
            }
        }

        #endregion
    }

    public class SheetEllipseMode
    {
        #region IoC

        private readonly IServiceLocator _serviceLocator;
        private readonly IBlockController _blockController;
        private readonly IBlockFactory _blockFactory;
        private readonly IBlockHelper _blockHelper;
        private readonly IItemController _itemController;
        private readonly IPointController _pointController;

        private readonly SheetState _state;

        public SheetEllipseMode(IServiceLocator serviceLocator, SheetState state)
        {
            this._serviceLocator = serviceLocator;
            this._blockController = serviceLocator.GetInstance<IBlockController>();
            this._blockFactory = serviceLocator.GetInstance<IBlockFactory>();
            this._blockHelper = serviceLocator.GetInstance<IBlockHelper>();
            this._itemController = serviceLocator.GetInstance<IItemController>();
            this._pointController = serviceLocator.GetInstance<IPointController>();

            this._state = state;
        }

        #endregion

        #region Fields

        private IEllipse TempEllipse;
        private ImmutablePoint SelectionStartPoint;

        #endregion

        #region Methods

        public void Init(ImmutablePoint p)
        {
            double x = _itemController.Snap(p.X, _state.Options.SnapSize);
            double y = _itemController.Snap(p.Y, _state.Options.SnapSize);
            SelectionStartPoint = new ImmutablePoint(x, y);
            TempEllipse = _blockFactory.CreateEllipse(_state.Options.LineThickness / _state.ZoomController.Zoom, x, y, 0.0, 0.0, false, ItemColors.Black, ItemColors.Transparent);
            _state.OverlaySheet.Add(TempEllipse);
            _state.OverlaySheet.Capture();
        }

        public void Move(ImmutablePoint p)
        {
            double sx = SelectionStartPoint.X;
            double sy = SelectionStartPoint.Y;
            double x = _itemController.Snap(p.X, _state.Options.SnapSize);
            double y = _itemController.Snap(p.Y, _state.Options.SnapSize);
            _blockHelper.SetLeft(TempEllipse, Math.Min(sx, x));
            _blockHelper.SetTop(TempEllipse, Math.Min(sy, y));
            _blockHelper.SetWidth(TempEllipse, Math.Abs(sx - x));
            _blockHelper.SetHeight(TempEllipse, Math.Abs(sy - y));
        }

        public void Finish()
        {
            double x = _blockHelper.GetLeft(TempEllipse);
            double y = _blockHelper.GetTop(TempEllipse);
            double width = _blockHelper.GetWidth(TempEllipse);
            double height = _blockHelper.GetHeight(TempEllipse);
            if (width == 0.0 || height == 0.0)
            {
                Cancel();
            }
            else
            {
                _state.OverlaySheet.ReleaseCapture();
                _state.OverlaySheet.Remove(TempEllipse);
                _state.HistoryController.Register("Create Ellipse");
                _state.ContentBlock.Ellipses.Add(TempEllipse);
                _state.ContentSheet.Add(TempEllipse);
                TempEllipse = null;
            }
        }

        public void Cancel()
        {
            _state.OverlaySheet.ReleaseCapture();
            _state.OverlaySheet.Remove(TempEllipse);
            TempEllipse = null;
        }

        public void Reset()
        {
            if (TempEllipse != null)
            {
                _state.OverlaySheet.Remove(TempEllipse);
                TempEllipse = null;
            }
        }

        public void Adjust(double zoom)
        {
            double lineThicknessZoomed = _state.Options.LineThickness / zoom;

            if (TempEllipse != null)
            {
                _blockHelper.SetStrokeThickness(TempEllipse, lineThicknessZoomed);
            }
        }

        public void ToggleFill()
        {
            if (TempEllipse != null)
            {
                _blockController.ToggleFill(TempEllipse);
            }
        }

        #endregion
    }

    public static class FileDialogSettings
    {
        #region Extensions

        public static string SolutionExtension = ".solution";
        public static string DocumentExtension = ".document";
        public static string PageExtension = ".page";
        public static string LibraryExtension = ".library";

        public static string DatabaseExtension = ".csv";

        public static string JsonSolutionExtension = ".jsolution";
        public static string JsonDocumentExtension = ".jdocument";
        public static string JsonPageExtension = ".jpage";
        public static string JsonLibraryExtension = ".jlibrary";

        #endregion

        #region Filters

        public static string SolutionFilter = "Solution Files (*.solution)|*.solution|Json Solution Files (*.jsolution)|*.jsolution|All Files (*.*)|*.*";
        public static string DocumentFilter = "Document Files (*.document)|*.document|Json Document Files (*.jdocument)|*.jdocument|All Files (*.*)|*.*";
        public static string PageFilter = "Page Files (*.page)|*.page|Json Page Files (*.jpage)|*.jpage|All Files (*.*)|*.*";
        public static string LibraryFilter = "Library Files (*.library)|*.library|Json Library Files (*.jlibrary)|*.jlibrary|All Files (*.*)|*.*";

        public static string DatabaseFilter = "Csv Files (*.csv)|*.csv|All Files (*.*)|*.*";
        public static string ImageFilter = "Supported Files (*.png;*.jpg;*.jpeg)|*.png;*.jpg;*.jpeg|Png Files (*.png)|*.png|Jpg Files (*.jpg)|*.jpg|Jpeg Files (*.jpeg)|*.jpeg|All Files (*.*)|*.*";
        public static string ExportFilter = "Pdf Documents (*.pdf)|*.pdf|Dxf Documents (*.dxf)|*.dxf|All Files (*.*)|*.*";

        #endregion
    }

    public class SheetController : ISheetController
    {
        #region IoC

        private readonly IServiceLocator _serviceLocator;
        private readonly IBlockController _blockController;
        private readonly IBlockFactory _blockFactory;
        private readonly IBlockSerializer _blockSerializer;
        private readonly IBlockHelper _blockHelper;
        private readonly IItemController _itemController;
        private readonly IItemSerializer _itemSerializer;
        private readonly IJsonSerializer _jsonSerializer;
        private readonly IClipboard _clipboard;
        private readonly IBase64 _base64;
        private readonly IPointController _pointController;
        private readonly IPageFactory _pageFactory;

        private readonly SheetLineMode _lineMode;
        private readonly SheetRectangleMode _rectangleMode;
        private readonly SheetEllipseMode _ellipseMode;

        public SheetController(IServiceLocator serviceLocator)
        {
            this._serviceLocator = serviceLocator;
            this._blockController = serviceLocator.GetInstance<IBlockController>();
            this._blockFactory = serviceLocator.GetInstance<IBlockFactory>();
            this._blockSerializer = serviceLocator.GetInstance<IBlockSerializer>();
            this._blockHelper = serviceLocator.GetInstance<IBlockHelper>();
            this._itemController = serviceLocator.GetInstance<IItemController>();
            this._itemSerializer = serviceLocator.GetInstance<IItemSerializer>();
            this._jsonSerializer = serviceLocator.GetInstance<IJsonSerializer>();
            this._clipboard = serviceLocator.GetInstance<IClipboard>();
            this._base64 = serviceLocator.GetInstance<IBase64>();
            this._pointController = serviceLocator.GetInstance<IPointController>();
            this._pageFactory = serviceLocator.GetInstance<IPageFactory>();

            this.State = DefaultState;

            this._lineMode = new SheetLineMode(_serviceLocator, this.State);
            this._rectangleMode = new SheetRectangleMode(_serviceLocator, this.State);
            this._ellipseMode = new SheetEllipseMode(_serviceLocator, this.State);
        }

        #endregion

        #region Properties

        public SheetState State { get; set; }

        public static SheetState DefaultState
        {
            get
            {
                return new SheetState()
                {
                    Mode = SheetMode.Selection,
                    TempMode = SheetMode.None,
                    IsFirstMove = true,
                    SelectedType = ItemType.None
                };
            }
        }

        #endregion

        #region ToSingle

        public static IEnumerable<T> ToSingle<T>(T item)
        {
            yield return item;
        }

        #endregion

        #region Init

        public void Init()
        {
            SetDefaults();

            CreateBlocks();
            CreatePlugins();
            CreatePage();

            LoadLibraryFromResource(string.Concat("Sheet.Libraries", '.', "Digital.library"));
        }

        private void SetDefaults()
        {
            State.Options = new SheetOptions()
            {
                PageOriginX = 0.0,
                PageOriginY = 0.0,
                PageWidth = 1260.0,
                PageHeight = 891.0,
                SnapSize = 15,
                GridSize = 30,
                FrameThickness = 1.0,
                GridThickness = 1.0,
                SelectionThickness = 1.0,
                LineThickness = 2.0,
                HitTestSize = 3.5,
                DefaultZoomIndex = 9,
                MaxZoomIndex = 21,
                ZoomFactors = new double[] { 0.01, 0.0625, 0.0833, 0.125, 0.25, 0.3333, 0.5, 0.6667, 0.75, 1, 1.25, 1.5, 2, 3, 4, 6, 8, 12, 16, 24, 32, 64 }
            };

            State.ZoomController.ZoomIndex = State.Options.DefaultZoomIndex;
        }

        private void CreateBlocks()
        {
            State.ContentBlock = CreateContentBlock();
            State.FrameBlock = CreateFrameBlock();
            State.GridBlock = CreateGridBlock();
            State.SelectedBlock = CreateSelectedBlock();
        }

        #endregion

        #region Blocks

        private IBlock CreateContentBlock()
        {
            return _blockFactory.CreateBlock(-1, State.Options.PageOriginX, State.Options.PageOriginY, State.Options.PageWidth, State.Options.PageHeight, -1, "CONTENT", null);
        }

        private IBlock CreateFrameBlock()
        {
            return _blockFactory.CreateBlock(-1, State.Options.PageOriginX, State.Options.PageOriginY, State.Options.PageWidth, State.Options.PageHeight, -1, "FRAME", null);
        }

        private IBlock CreateGridBlock()
        {
            return _blockFactory.CreateBlock(-1, State.Options.PageOriginX, State.Options.PageOriginY, State.Options.PageWidth, State.Options.PageHeight, -1, "GRID", null);
        }

        private IBlock CreateSelectedBlock()
        {
            return _blockFactory.CreateBlock(-1, State.Options.PageOriginX, State.Options.PageOriginY, State.Options.PageWidth, State.Options.PageHeight, -1, "SELECTED", null);
        }

        private IBlock CreateTempBlock()
        {
            return _blockFactory.CreateBlock(-1, State.Options.PageOriginX, State.Options.PageOriginY, State.Options.PageWidth, State.Options.PageHeight, -1, "TEMP", null);
        }

        #endregion

        #region Page

        public async void SetPage(string text)
        {
            try
            {
                if (text == null)
                {
                    State.HistoryController.Reset();
                    ResetPage();
                }
                else
                {
                    var block = await Task.Run(() => _itemSerializer.DeserializeContents(text));
                    State.HistoryController.Reset();
                    ResetPage();
                    DeserializePage(block);
                }
            }
            catch (Exception ex)
            {
                Debug.Print(ex.Message);
                Debug.Print(ex.StackTrace);
            }
        }

        public string GetPage()
        {
            var block = SerializePage();
            var text = _itemSerializer.SerializeContents(block);

            return text;
        }

        public void ExportPage(string text)
        {
            var block = _itemSerializer.DeserializeContents(text);
            Export(ToSingle(block));
        }

        public void ExportPages(IEnumerable<string> texts)
        {
            var blocks = texts.Select(text => _itemSerializer.DeserializeContents(text));
            Export(blocks);
        }

        public BlockItem SerializePage()
        {
            _blockController.Deselect(State.SelectedBlock);
            State.SelectedBlock = CreateSelectedBlock();

            var grid = _blockSerializer.SerializerAndSetId(State.GridBlock, -1, State.GridBlock.X, State.GridBlock.Y, State.GridBlock.Width, State.GridBlock.Height, -1, "GRID");
            var frame = _blockSerializer.SerializerAndSetId(State.FrameBlock, -1, State.FrameBlock.X, State.FrameBlock.Y, State.FrameBlock.Width, State.FrameBlock.Height, -1, "FRAME");
            var content = _blockSerializer.SerializerAndSetId(State.ContentBlock, -1, State.ContentBlock.X, State.ContentBlock.Y, State.ContentBlock.Width, State.ContentBlock.Height, -1, "CONTENT");

            var page = new BlockItem(-1, State.Options.PageOriginX, State.Options.PageOriginY, State.Options.PageWidth, State.Options.PageHeight, -1, "PAGE", new ItemColor() { Alpha = 0, Red = 0, Green = 0, Blue = 0 });
            page.Blocks.Add(grid);
            page.Blocks.Add(frame);
            page.Blocks.Add(content);

            return page;
        }

        public void DeserializePage(BlockItem page)
        {
            BlockItem grid = page.Blocks.Where(block => block.Name == "GRID").FirstOrDefault();
            BlockItem frame = page.Blocks.Where(block => block.Name == "FRAME").FirstOrDefault();
            BlockItem content = page.Blocks.Where(block => block.Name == "CONTENT").FirstOrDefault();

            if (grid != null)
            {
                _blockController.AddContents(State.BackSheet, grid, State.GridBlock, null, false, State.Options.GridThickness / State.ZoomController.Zoom);
            }

            if (frame != null)
            {
                _blockController.AddContents(State.BackSheet, frame, State.FrameBlock, null, false, State.Options.FrameThickness / State.ZoomController.Zoom);
            }

            if (content != null)
            {
                _blockController.AddContents(State.ContentSheet, content, State.ContentBlock, null, false, State.Options.LineThickness / State.ZoomController.Zoom);
            }
        }

        public void ResetPage()
        {
            ResetOverlay();

            _blockController.Remove(State.BackSheet, State.GridBlock);
            _blockController.Remove(State.BackSheet, State.FrameBlock);
            _blockController.Remove(State.ContentSheet, State.ContentBlock);

            CreateBlocks();
        }

        public void ResetPageContent()
        {
            ResetOverlay();

            _blockController.Remove(State.ContentSheet, State.ContentBlock);
        }

        #endregion

        #region Clipboard

        public void CutText()
        {
            try
            {
                if (_blockController.HaveSelected(State.SelectedBlock))
                {
                    var copy = _blockFactory.CreateBlock(State.SelectedBlock.Id, State.SelectedBlock.X, State.SelectedBlock.Y, State.SelectedBlock.Width, State.SelectedBlock.Height, State.SelectedBlock.DataId, State.SelectedBlock.Name, null);
                    _blockController.ShallowCopy(State.SelectedBlock, copy);
                    State.HistoryController.Register("Cut");
                    CopyText(copy);
                    Delete(copy);
                }
            }
            catch (Exception ex)
            {
                Debug.Print(ex.Message);
                Debug.Print(ex.StackTrace);
            }
        }

        private void CopyText(IBlock block)
        {
            try
            {
                var selected = _blockSerializer.SerializerAndSetId(block, -1, 0.0, 0.0, 0.0, 0.0, -1, "SELECTED");
                var text = _itemSerializer.SerializeContents(selected);
                _clipboard.Set(text);
            }
            catch (Exception ex)
            {
                Debug.Print(ex.Message);
                Debug.Print(ex.StackTrace);
            }
        }

        public void CopyText()
        {
            if (_blockController.HaveSelected(State.SelectedBlock))
            {
                CopyText(State.SelectedBlock);
            }
        }

        public async void PasteText()
        {
            try
            {
                var text = _clipboard.Get();
                var block = await Task.Run(() => _itemSerializer.DeserializeContents(text));
                State.HistoryController.Register("Paste");
                InsertContent(block, true);
            }
            catch (Exception ex)
            {
                Debug.Print(ex.Message);
                Debug.Print(ex.StackTrace);
            }
        }

        public void CutJson()
        {
            try
            {
                if (_blockController.HaveSelected(State.SelectedBlock))
                {
                    var copy = _blockFactory.CreateBlock(State.SelectedBlock.Id, State.SelectedBlock.X, State.SelectedBlock.Y, State.SelectedBlock.Width, State.SelectedBlock.Height, State.SelectedBlock.DataId, State.SelectedBlock.Name, null);
                    _blockController.ShallowCopy(State.SelectedBlock, copy);
                    State.HistoryController.Register("Cut");
                    CopyJson(copy);
                    Delete(copy);
                }
            }
            catch (Exception ex)
            {
                Debug.Print(ex.Message);
                Debug.Print(ex.StackTrace);
            }
        }

        private void CopyJson(IBlock block)
        {
            try
            {
                var selected = _blockSerializer.SerializerAndSetId(block, -1, 0.0, 0.0, 0.0, 0.0, -1, "SELECTED");
                string json = _jsonSerializer.Serialize(selected);
                _clipboard.Set(json);
            }
            catch (Exception ex)
            {
                Debug.Print(ex.Message);
                Debug.Print(ex.StackTrace);
            }
        }

        public void CopyJson()
        {
            if (_blockController.HaveSelected(State.SelectedBlock))
            {
                CopyJson(State.SelectedBlock);
            }
        }

        public async void PasteJson()
        {
            try
            {
                var text = _clipboard.Get();
                var block = await Task.Run(() => _jsonSerializer.Deerialize<BlockItem>(text));
                State.HistoryController.Register("Paste");
                InsertContent(block, true);
            }
            catch (Exception ex)
            {
                Debug.Print(ex.Message);
                Debug.Print(ex.StackTrace);
            }
        }

        #endregion

        #region Overlay

        private void ResetOverlay()
        {
            _lineMode.Reset();
            _rectangleMode.Reset();
            _ellipseMode.Reset();

            if (State.TempSelectionRect != null)
            {
                State.OverlaySheet.Remove(State.TempSelectionRect);
                State.TempSelectionRect = null;
            }

            if (State.LineThumbStart != null)
            {
                State.OverlaySheet.Remove(State.LineThumbStart);
            }

            if (State.LineThumbEnd != null)
            {
                State.OverlaySheet.Remove(State.LineThumbEnd);
            }
        }

        #endregion

        #region Delete

        public void Delete(IBlock block)
        {
            FinishEdit();
            _blockController.RemoveSelected(State.ContentSheet, State.ContentBlock, block);
        }

        public void Delete()
        {
            if (_blockController.HaveSelected(State.SelectedBlock))
            {
                var copy = _blockFactory.CreateBlock(State.SelectedBlock.Id, State.SelectedBlock.X, State.SelectedBlock.Y, State.SelectedBlock.Width, State.SelectedBlock.Height, State.SelectedBlock.DataId, State.SelectedBlock.Name, null);
                _blockController.ShallowCopy(State.SelectedBlock, copy);
                State.HistoryController.Register("Delete");
                Delete(copy);
            }
        }

        #endregion

        #region Select All

        public void SelecteAll()
        {
            State.SelectedBlock = CreateSelectedBlock();
            _blockController.ShallowCopy(State.ContentBlock, State.SelectedBlock);
            _blockController.Select(State.ContentBlock);
        }

        #endregion

        #region Deselect All

        public void DeselectAll()
        {
            State.SelectedBlock = CreateSelectedBlock();
            _blockController.Deselect(State.ContentBlock);
        }

        #endregion

        #region Toggle Fill

        public void ToggleFill()
        {
            switch (State.Mode)
            {
                case SheetMode.Line:
                    _lineMode.ToggleFill();
                    break;
                case SheetMode.Rectangle:
                    _rectangleMode.ToggleFill();
                    break;
                case SheetMode.Ellipse:
                    _ellipseMode.ToggleFill();
                    break;
            }
        }

        #endregion

        #region Insert Mode

        private void InsertContent(BlockItem block, bool select)
        {
            _blockController.Deselect(State.SelectedBlock);
            State.SelectedBlock = CreateSelectedBlock();
            _blockController.AddContents(State.ContentSheet, block, State.ContentBlock, State.SelectedBlock, select, State.Options.LineThickness / State.ZoomController.Zoom);
        }

        private BlockItem CreateBlock(string name, IBlock block)
        {
            try
            {
                var blockItem = _blockSerializer.Serialize(block);
                blockItem.Name = name;
                return blockItem;
            }
            catch (Exception ex)
            {
                Debug.Print(ex.Message);
                Debug.Print(ex.StackTrace);
            }
            return null;
        }

        public void CreateBlock()
        {
            if (_blockController.HaveSelected(State.SelectedBlock))
            {
                State.TempMode = State.Mode;
                State.Mode = SheetMode.TextEditor;

                var tc = CreateTextEditor(new ImmutablePoint((State.EditorSheet.Width / 2) - (330 / 2), State.EditorSheet.Height / 2));

                Action<string> ok = (name) =>
                {
                    var block = CreateBlock(name, State.SelectedBlock);
                    if (block != null)
                    {
                        AddToLibrary(block);
                    }
                    State.EditorSheet.Remove(tc);
                    State.View.Focus();
                    State.Mode = State.TempMode;
                };

                Action cancel = () =>
                {
                    State.EditorSheet.Remove(tc);
                    State.View.Focus();
                    State.Mode = State.TempMode;
                };

                tc.Set(ok, cancel, "Create Block", "Name:", "BLOCK0");
                State.EditorSheet.Add(tc);
            }
        }

        public async void BreakBlock()
        {
            if (_blockController.HaveSelected(State.SelectedBlock))
            {
                var text = _itemSerializer.SerializeContents(_blockSerializer.SerializerAndSetId(State.SelectedBlock, 0, 0.0, 0.0, 0.0, 0.0, -1, "SELECTED"));
                var block = await Task.Run(() => _itemSerializer.DeserializeContents(text));
                State.HistoryController.Register("Break Block");
                Delete();
                _blockController.AddBroken(State.ContentSheet, block, State.ContentBlock, State.SelectedBlock, true, State.Options.LineThickness / State.ZoomController.Zoom);
            }
        }

        #endregion

        #region Point Mode

        public IPoint InsertPoint(ImmutablePoint p, bool register, bool select)
        {
            double thickness = State.Options.LineThickness / State.ZoomController.Zoom;
            double x = _itemController.Snap(p.X, State.Options.SnapSize);
            double y = _itemController.Snap(p.Y, State.Options.SnapSize);

            var point = _blockFactory.CreatePoint(thickness, x, y, false);

            if (register)
            {
                _blockController.Deselect(State.SelectedBlock);
                State.SelectedBlock = CreateSelectedBlock();
                State.HistoryController.Register("Insert Point");
            }

            State.ContentBlock.Points.Add(point);
            State.ContentSheet.Add(point);

            if (select)
            {
                State.SelectedBlock.Points = new List<IPoint>();
                State.SelectedBlock.Points.Add(point);

                _blockController.Select(point);
            }

            return point;
        }

        #endregion

        #region Move Mode

        private void Move(double x, double y)
        {
            if (_blockController.HaveSelected(State.SelectedBlock))
            {
                var copy = _blockFactory.CreateBlock(State.SelectedBlock.Id, State.SelectedBlock.X, State.SelectedBlock.Y, State.SelectedBlock.Width, State.SelectedBlock.Height, State.SelectedBlock.DataId, State.SelectedBlock.Name, null);
                _blockController.ShallowCopy(State.SelectedBlock, copy);
                FinishEdit();
                State.HistoryController.Register("Move");
                _blockController.Select(copy);
                State.SelectedBlock = copy;
                _blockController.MoveDelta(x, y, State.SelectedBlock);
            }
        }

        public void MoveUp()
        {
            Move(0.0, -State.Options.SnapSize);
        }

        public void MoveDown()
        {
            Move(0.0, State.Options.SnapSize);
        }

        public void MoveLeft()
        {
            Move(-State.Options.SnapSize, 0.0);
        }

        public void MoveRight()
        {
            Move(State.Options.SnapSize, 0.0);
        }

        private bool CanInitMove(ImmutablePoint p)
        {
            var temp = CreateTempBlock();
            _blockController.HitTest(State.ContentSheet, State.SelectedBlock, p, State.Options.HitTestSize, temp, false, false);
            if (_blockController.HaveSelected(temp))
            {
                return true;
            }
            return false;
        }

        private void InitMove(ImmutablePoint p)
        {
            State.IsFirstMove = true;
            State.TempMode = State.Mode;
            State.Mode = SheetMode.Move;
            double x = _itemController.Snap(p.X, State.Options.SnapSize);
            double y = _itemController.Snap(p.Y, State.Options.SnapSize);
            State.PanStartPoint = new ImmutablePoint(x, y);
            ResetOverlay();
            State.OverlaySheet.Capture();
        }

        private void Move(ImmutablePoint p)
        {
            if (State.IsFirstMove)
            {
                var copy = _blockFactory.CreateBlock(State.SelectedBlock.Id, State.SelectedBlock.X, State.SelectedBlock.Y, State.SelectedBlock.Width, State.SelectedBlock.Height, State.SelectedBlock.DataId, State.SelectedBlock.Name, null);
                _blockController.ShallowCopy(State.SelectedBlock, copy);
                State.HistoryController.Register("Move");
                State.IsFirstMove = false;
                State.CursorController.Set(SheetCursor.Move);
                _blockController.Select(copy);
                State.SelectedBlock = copy;
            }

            double x = _itemController.Snap(p.X, State.Options.SnapSize);
            double y = _itemController.Snap(p.Y, State.Options.SnapSize);
            double dx = x - State.PanStartPoint.X;
            double dy = y - State.PanStartPoint.Y;

            if (dx != 0.0 || dy != 0.0)
            {
                _blockController.MoveDelta(dx, dy, State.SelectedBlock);
                State.PanStartPoint = new ImmutablePoint(x, y);
            }
        }

        private void FinishMove()
        {
            State.Mode = State.TempMode;
            State.CursorController.Set(SheetCursor.Normal);
            State.OverlaySheet.ReleaseCapture();
        }

        #endregion

        #region Pan & Zoom Mode

        public void SetAutoFitSize(double finalWidth, double finalHeight)
        {
            State.LastFinalWidth = finalWidth;
            State.LastFinalHeight = finalHeight;
        }

        private void ZoomTo(double x, double y, int oldZoomIndex)
        {
            double oldZoom = GetZoom(oldZoomIndex);
            double newZoom = GetZoom(State.ZoomController.ZoomIndex);
            State.ZoomController.Zoom = newZoom;

            State.ZoomController.PanX = (x * oldZoom + State.ZoomController.PanX) - x * newZoom;
            State.ZoomController.PanY = (y * oldZoom + State.ZoomController.PanY) - y * newZoom;
        }

        private void ZoomTo(int delta, ImmutablePoint p)
        {
            if (delta > 0)
            {
                if (State.ZoomController.ZoomIndex > -1 && State.ZoomController.ZoomIndex < State.Options.MaxZoomIndex)
                {
                    ZoomTo(p.X, p.Y, State.ZoomController.ZoomIndex++);
                }
            }
            else
            {
                if (State.ZoomController.ZoomIndex > 0)
                {
                    ZoomTo(p.X, p.Y, State.ZoomController.ZoomIndex--);
                }
            }
        }

        private double GetZoom(int index)
        {
            if (index >= 0 && index <= State.Options.MaxZoomIndex)
            {
                return State.Options.ZoomFactors[index];
            }
            return State.ZoomController.Zoom;
        }

        private void InitPan(ImmutablePoint p)
        {
            State.TempMode = State.Mode;
            State.Mode = SheetMode.Pan;
            State.PanStartPoint = new ImmutablePoint(p.X, p.Y);
            ResetOverlay();
            State.CursorController.Set(SheetCursor.Pan);
            State.OverlaySheet.Capture();
        }

        private void Pan(ImmutablePoint p)
        {
            State.ZoomController.PanX = State.ZoomController.PanX + p.X - State.PanStartPoint.X;
            State.ZoomController.PanY = State.ZoomController.PanY + p.Y - State.PanStartPoint.Y;
            State.PanStartPoint = new ImmutablePoint(p.X, p.Y);
        }

        private void FinishPan()
        {
            State.Mode = State.TempMode;
            State.CursorController.Set(SheetCursor.Normal);
            State.OverlaySheet.ReleaseCapture();
        }

        private void AdjustThickness(IEnumerable<ILine> lines, double thickness)
        {
            foreach (var line in lines)
            {
                _blockHelper.SetStrokeThickness(line, thickness);
            }
        }

        private void AdjustThickness(IEnumerable<IRectangle> rectangles, double thickness)
        {
            foreach (var rectangle in rectangles)
            {
                _blockHelper.SetStrokeThickness(rectangle, thickness);
            }
        }

        private void AdjustThickness(IEnumerable<IEllipse> ellipses, double thickness)
        {
            foreach (var ellipse in ellipses)
            {
                _blockHelper.SetStrokeThickness(ellipse, thickness);
            }
        }

        private void AdjustThickness(IBlock parent, double thickness)
        {
            if (parent != null)
            {
                AdjustThickness(parent.Lines, thickness);
                AdjustThickness(parent.Rectangles, thickness);
                AdjustThickness(parent.Ellipses, thickness);

                foreach (var block in parent.Blocks)
                {
                    AdjustThickness(block, thickness);
                }
            }
        }

        public void AdjustBackThickness(double zoom)
        {
            double gridThicknessZoomed = State.Options.GridThickness / zoom;
            double frameThicknessZoomed = State.Options.FrameThickness / zoom;

            AdjustThickness(State.GridBlock, gridThicknessZoomed);
            AdjustThickness(State.FrameBlock, frameThicknessZoomed);
        }

        public void AdjustPageThickness(double zoom)
        {
            double lineThicknessZoomed = State.Options.LineThickness / zoom;
            double selectionThicknessZoomed = State.Options.SelectionThickness / zoom;

            AdjustBackThickness(zoom);
            AdjustThickness(State.ContentBlock, lineThicknessZoomed);

            _lineMode.Adjust(zoom);
            _rectangleMode.Adjust(zoom);
            _ellipseMode.Adjust(zoom);

            if (State.TempSelectionRect != null)
            {
                _blockHelper.SetStrokeThickness(State.TempSelectionRect, selectionThicknessZoomed);
            }
        }

        #endregion

        #region Selection Mode

        private void InitSelectionRect(ImmutablePoint p)
        {
            State.SelectionStartPoint = new ImmutablePoint(p.X, p.Y);
            State.TempSelectionRect = _pageFactory.CreateSelectionRectangle(State.Options.SelectionThickness / State.ZoomController.Zoom, p.X, p.Y, 0.0, 0.0);
            State.OverlaySheet.Add(State.TempSelectionRect);
            State.OverlaySheet.Capture();
        }

        private void MoveSelectionRect(ImmutablePoint p)
        {
            double sx = State.SelectionStartPoint.X;
            double sy = State.SelectionStartPoint.Y;
            double x = p.X;
            double y = p.Y;
            double width = Math.Abs(sx - x);
            double height = Math.Abs(sy - y);
            _blockHelper.SetLeft(State.TempSelectionRect, Math.Min(sx, x));
            _blockHelper.SetTop(State.TempSelectionRect, Math.Min(sy, y));
            _blockHelper.SetWidth(State.TempSelectionRect, width);
            _blockHelper.SetHeight(State.TempSelectionRect, height);
        }

        private void FinishSelectionRect()
        {
            double x = _blockHelper.GetLeft(State.TempSelectionRect);
            double y = _blockHelper.GetTop(State.TempSelectionRect);
            double width = _blockHelper.GetWidth(State.TempSelectionRect);
            double height = _blockHelper.GetHeight(State.TempSelectionRect);

            CancelSelectionRect();

            State.SelectedBlock = CreateSelectedBlock();
            _blockController.HitTest(State.ContentSheet, State.ContentBlock, new ImmutableRect(x, y, width, height), State.SelectedBlock, false, false);
            _blockController.Select(State.SelectedBlock);

            TryToEditSelected();
        }

        private void CancelSelectionRect()
        {
            State.OverlaySheet.ReleaseCapture();
            State.OverlaySheet.Remove(State.TempSelectionRect);
            State.TempSelectionRect = null;
        }

        #endregion

        #region Text Mode

        private TextControl CreateTextEditor(ImmutablePoint p)
        {
            var tc = new TextControl() { Width = 330.0, Background = Brushes.WhiteSmoke };
            tc.RenderTransform = null;
            Canvas.SetLeft(tc, p.X);
            Canvas.SetTop(tc, p.Y);
            return tc;
        }

        private void CreateText(ImmutablePoint p)
        {
            double x = _itemController.Snap(p.X, State.Options.SnapSize);
            double y = _itemController.Snap(p.Y, State.Options.SnapSize);
            State.HistoryController.Register("Create Text");

            var text = _blockFactory.CreateText("Text", x, y, 30.0, 15.0, (int)XHorizontalAlignment.Center, (int)XVerticalAlignment.Center, 11.0, ItemColors.Transparent, ItemColors.Black);
            State.ContentBlock.Texts.Add(text);
            State.ContentSheet.Add(text);
        }

        private bool TryToEditText(ImmutablePoint p)
        {
            var temp = CreateTempBlock();
            _blockController.HitTest(State.ContentSheet, State.ContentBlock, p, State.Options.HitTestSize, temp, true, true);

            // check if one text is selected and ignore selected blocks
            if (temp.Points.Count == 0
                && temp.Lines.Count == 0
                && temp.Rectangles.Count == 0
                && temp.Ellipses.Count == 0
                && temp.Texts.Count == 1
                && temp.Images.Count == 0)
            {
                var tb = WpfBlockHelper.GetTextBlock(temp.Texts[0]);

                State.TempMode = State.Mode;
                State.Mode = SheetMode.TextEditor;

                var tc = CreateTextEditor(new ImmutablePoint((State.EditorSheet.Width / 2) - (330 / 2), State.EditorSheet.Height / 2) /* p */);

                Action<string> ok = (text) =>
                {
                    State.HistoryController.Register("Edit Text");
                    tb.Text = text;
                    State.EditorSheet.Remove(tc);
                    State.View.Focus();
                    State.Mode = State.TempMode;
                };

                Action cancel = () =>
                {
                    State.EditorSheet.Remove(tc);
                    State.View.Focus();
                    State.Mode = State.TempMode;
                };

                tc.Set(ok, cancel, "Edit Text", "Text:", tb.Text);
                State.EditorSheet.Add(tc);

                _blockController.Deselect(temp);
                return true;
            }

            _blockController.Deselect(temp);
            return false;
        }

        #endregion

        #region Image Mode

        private void Image(ImmutablePoint p)
        {
            var dlg = _serviceLocator.GetInstance<IOpenFileDialog>();
            dlg.Filter = FileDialogSettings.ImageFilter;
            dlg.FilterIndex = 1;
            dlg.FileName = "";

            if (dlg.ShowDialog() == true)
            {
                try
                {
                    InsertImage(p, dlg.FileName);
                }
                catch (Exception ex)
                {
                    Debug.Print(ex.Message);
                    Debug.Print(ex.StackTrace);
                }
            }
        }

        private void InsertImage(ImmutablePoint p, string path)
        {
            byte[] data = _base64.ReadAllBytes(path);
            double x = _itemController.Snap(p.X, State.Options.SnapSize);
            double y = _itemController.Snap(p.Y, State.Options.SnapSize);
            var image = _blockFactory.CreateImage(x, y, 120.0, 90.0, data);
            State.ContentBlock.Images.Add(image);
            State.ContentSheet.Add(image);
        }

        #endregion

        #region Edit Mode

        private bool TryToEditSelected()
        {
            if (_blockController.HaveOnlyOneLineSelected(State.SelectedBlock))
            {
                InitLineEditor();
                return true;
            }
            else if (_blockController.HaveOnlyOneRectangleSelected(State.SelectedBlock))
            {
                InitRectangleEditor();
                return true;
            }
            else if (_blockController.HaveOnlyOneEllipseSelected(State.SelectedBlock))
            {
                InitEllipseEditor();
                return true;
            }
            else if (_blockController.HaveOnlyOneTextSelected(State.SelectedBlock))
            {
                InitTextEditor();
                return true;
            }
            else if (_blockController.HaveOnlyOneImageSelected(State.SelectedBlock))
            {
                InitImageEditor();
                return true;
            }
            return false;
        }

        private void FinishEdit()
        {
            switch (State.SelectedType)
            {
                case ItemType.Line:
                    FinishLineEditor();
                    break;
                case ItemType.Rectangle:
                case ItemType.Ellipse:
                case ItemType.Text:
                case ItemType.Image:
                    FinishFrameworkElementEditor();
                    break;
            }
        }

        #endregion

        #region Edit Line

        private void DragLineStart(ILine line, IThumb thumb, double dx, double dy)
        {
            if (line != null && thumb != null)
            {
                if (line.Start != null)
                {
                    double x = _itemController.Snap(line.Start.X + dx, State.Options.SnapSize);
                    double y = _itemController.Snap(line.Start.Y + dy, State.Options.SnapSize);
                    double sdx = x - line.Start.X;
                    double sdy = y - line.Start.Y;
                    _blockController.MoveDelta(sdx, sdy, line.Start);
                    _blockHelper.SetLeft(thumb, x);
                    _blockHelper.SetTop(thumb, y);
                }
                else
                {
                    double x = _itemController.Snap(_blockHelper.GetX1(line) + dx, State.Options.SnapSize);
                    double y = _itemController.Snap(_blockHelper.GetY1(line) + dy, State.Options.SnapSize);
                    _blockHelper.SetX1(line, x);
                    _blockHelper.SetY1(line, y);
                    _blockHelper.SetLeft(thumb, x);
                    _blockHelper.SetTop(thumb, y);
                }
            }
        }

        private void DragLineEnd(ILine line, IThumb thumb, double dx, double dy)
        {
            if (line != null && thumb != null)
            {
                if (line.End != null)
                {
                    double x = _itemController.Snap(line.End.X + dx, State.Options.SnapSize);
                    double y = _itemController.Snap(line.End.Y + dy, State.Options.SnapSize);
                    double sdx = x - line.End.X;
                    double sdy = y - line.End.Y;
                    _blockController.MoveDelta(sdx, sdy, line.End);
                    _blockHelper.SetLeft(thumb, x);
                    _blockHelper.SetTop(thumb, y);
                }
                else
                {
                    double x = _itemController.Snap(_blockHelper.GetX2(line) + dx, State.Options.SnapSize);
                    double y = _itemController.Snap(_blockHelper.GetY2(line) + dy, State.Options.SnapSize);
                    _blockHelper.SetX2(line, x);
                    _blockHelper.SetY2(line, y);
                    _blockHelper.SetLeft(thumb, x);
                    _blockHelper.SetTop(thumb, y);
                }
            }
        }

        private void InitLineEditor()
        {
            State.TempMode = State.Mode;
            State.Mode = SheetMode.Edit;

            try
            {
                State.SelectedType = ItemType.Line;
                State.SelectedLine = State.SelectedBlock.Lines.FirstOrDefault();

                State.LineThumbStart = _blockFactory.CreateThumb(0.0, 0.0, State.SelectedLine, DragLineStart);
                State.LineThumbEnd = _blockFactory.CreateThumb(0.0, 0.0, State.SelectedLine, DragLineEnd);

                _blockHelper.SetLeft(State.LineThumbStart, _blockHelper.GetX1(State.SelectedLine));
                _blockHelper.SetTop(State.LineThumbStart, _blockHelper.GetY1(State.SelectedLine));
                _blockHelper.SetLeft(State.LineThumbEnd, _blockHelper.GetX2(State.SelectedLine));
                _blockHelper.SetTop(State.LineThumbEnd, _blockHelper.GetY2(State.SelectedLine));

                State.OverlaySheet.Add(State.LineThumbStart);
                State.OverlaySheet.Add(State.LineThumbEnd);
            }
            catch (Exception ex)
            {
                Debug.Print(ex.Message);
                Debug.Print(ex.StackTrace);
            }
        }

        private void FinishLineEditor()
        {
            State.Mode = State.TempMode;

            State.SelectedType = ItemType.None;
            State.SelectedLine = null;

            if (State.LineThumbStart != null)
            {
                State.OverlaySheet.Remove(State.LineThumbStart);
                State.LineThumbStart = null;
            }

            if (State.LineThumbEnd != null)
            {
                State.OverlaySheet.Remove(State.LineThumbEnd);
                State.LineThumbEnd = null;
            }
        }

        #endregion

        #region Edit FrameworkElement

        private void DragThumbs(Rect rect)
        {
            var tl = rect.TopLeft;
            var tr = rect.TopRight;
            var bl = rect.BottomLeft;
            var br = rect.BottomRight;

            _blockHelper.SetLeft(State.ThumbTopLeft, tl.X);
            _blockHelper.SetTop(State.ThumbTopLeft, tl.Y);
            _blockHelper.SetLeft(State.ThumbTopRight, tr.X);
            _blockHelper.SetTop(State.ThumbTopRight, tr.Y);
            _blockHelper.SetLeft(State.ThumbBottomLeft, bl.X);
            _blockHelper.SetTop(State.ThumbBottomLeft, bl.Y);
            _blockHelper.SetLeft(State.ThumbBottomRight, br.X);
            _blockHelper.SetTop(State.ThumbBottomRight, br.Y);
        }

        private void DragTopLeft(IElement element, IThumb thumb, double dx, double dy)
        {
            if (element != null && thumb != null)
            {
                double left = _blockHelper.GetLeft(element);
                double top = _blockHelper.GetTop(element);
                double width = _blockHelper.GetWidth(element);
                double height = _blockHelper.GetHeight(element);

                var rect = new Rect(left, top, width, height);
                rect.X = _itemController.Snap(rect.X + dx, State.Options.SnapSize);
                rect.Y = _itemController.Snap(rect.Y + dy, State.Options.SnapSize);
                rect.Width = Math.Max(0.0, rect.Width - (rect.X - left));
                rect.Height = Math.Max(0.0, rect.Height - (rect.Y - top));

                _blockHelper.SetLeft(element, rect.X);
                _blockHelper.SetTop(element, rect.Y);
                _blockHelper.SetWidth(element, rect.Width);
                _blockHelper.SetHeight(element, rect.Height);

                DragThumbs(rect);
            }
        }

        private void DragTopRight(IElement element, IThumb thumb, double dx, double dy)
        {
            if (element != null && thumb != null)
            {
                double left = _blockHelper.GetLeft(element);
                double top = _blockHelper.GetTop(element);
                double width = _blockHelper.GetWidth(element);
                double height = _blockHelper.GetHeight(element);

                var rect = new Rect(left, top, width, height);
                rect.Width = Math.Max(0.0, _itemController.Snap(rect.Width + dx, State.Options.SnapSize));
                rect.Y = _itemController.Snap(rect.Y + dy, State.Options.SnapSize);
                rect.Height = Math.Max(0.0, rect.Height - (rect.Y - top));

                _blockHelper.SetLeft(element, rect.X);
                _blockHelper.SetTop(element, rect.Y);
                _blockHelper.SetWidth(element, rect.Width);
                _blockHelper.SetHeight(element, rect.Height);

                DragThumbs(rect);
            }
        }

        private void DragBottomLeft(IElement element, IThumb thumb, double dx, double dy)
        {
            if (element != null && thumb != null)
            {
                double left = _blockHelper.GetLeft(element);
                double top = _blockHelper.GetTop(element);
                double width = _blockHelper.GetWidth(element);
                double height = _blockHelper.GetHeight(element);

                var rect = new Rect(left, top, width, height);
                rect.X = _itemController.Snap(rect.X + dx, State.Options.SnapSize);
                rect.Height = Math.Max(0.0, _itemController.Snap(rect.Height + dy, State.Options.SnapSize));
                rect.Width = Math.Max(0.0, rect.Width - (rect.X - left));

                _blockHelper.SetLeft(element, rect.X);
                _blockHelper.SetTop(element, rect.Y);
                _blockHelper.SetWidth(element, rect.Width);
                _blockHelper.SetHeight(element, rect.Height);

                DragThumbs(rect);
            }
        }

        private void DragBottomRight(IElement element, IThumb thumb, double dx, double dy)
        {
            if (element != null && thumb != null)
            {
                double left = _blockHelper.GetLeft(element);
                double top = _blockHelper.GetTop(element);
                double width = _blockHelper.GetWidth(element);
                double height = _blockHelper.GetHeight(element);

                var rect = new Rect(left, top, width, height);
                rect.Width = Math.Max(0.0, _itemController.Snap(rect.Width + dx, State.Options.SnapSize));
                rect.Height = Math.Max(0.0, _itemController.Snap(rect.Height + dy, State.Options.SnapSize));

                _blockHelper.SetLeft(element, rect.X);
                _blockHelper.SetTop(element, rect.Y);
                _blockHelper.SetWidth(element, rect.Width);
                _blockHelper.SetHeight(element, rect.Height);

                DragThumbs(rect);
            }
        }

        private void InitFrameworkElementEditor()
        {
            double left = _blockHelper.GetLeft(State.SelectedElement);
            double top = _blockHelper.GetTop(State.SelectedElement);
            double width = _blockHelper.GetWidth(State.SelectedElement);
            double height = _blockHelper.GetHeight(State.SelectedElement);

            State.ThumbTopLeft = _blockFactory.CreateThumb(0.0, 0.0, State.SelectedElement, DragTopLeft);
            State.ThumbTopRight = _blockFactory.CreateThumb(0.0, 0.0, State.SelectedElement, DragTopRight);
            State.ThumbBottomLeft = _blockFactory.CreateThumb(0.0, 0.0, State.SelectedElement, DragBottomLeft);
            State.ThumbBottomRight = _blockFactory.CreateThumb(0.0, 0.0, State.SelectedElement, DragBottomRight);

            _blockHelper.SetLeft(State.ThumbTopLeft, left);
            _blockHelper.SetTop(State.ThumbTopLeft, top);
            _blockHelper.SetLeft(State.ThumbTopRight, left + width);
            _blockHelper.SetTop(State.ThumbTopRight, top);
            _blockHelper.SetLeft(State.ThumbBottomLeft, left);
            _blockHelper.SetTop(State.ThumbBottomLeft, top + height);
            _blockHelper.SetLeft(State.ThumbBottomRight, left + width);
            _blockHelper.SetTop(State.ThumbBottomRight, top + height);

            State.OverlaySheet.Add(State.ThumbTopLeft);
            State.OverlaySheet.Add(State.ThumbTopRight);
            State.OverlaySheet.Add(State.ThumbBottomLeft);
            State.OverlaySheet.Add(State.ThumbBottomRight);
        }

        private void FinishFrameworkElementEditor()
        {
            State.Mode = State.TempMode;

            State.SelectedType = ItemType.None;
            State.SelectedElement = null;

            if (State.ThumbTopLeft != null)
            {
                State.OverlaySheet.Remove(State.ThumbTopLeft);
                State.ThumbTopLeft = null;
            }

            if (State.ThumbTopRight != null)
            {
                State.OverlaySheet.Remove(State.ThumbTopRight);
                State.ThumbTopRight = null;
            }

            if (State.ThumbBottomLeft != null)
            {
                State.OverlaySheet.Remove(State.ThumbBottomLeft);
                State.ThumbBottomLeft = null;
            }

            if (State.ThumbBottomRight != null)
            {
                State.OverlaySheet.Remove(State.ThumbBottomRight);
                State.ThumbBottomRight = null;
            }
        }

        #endregion

        #region Edit Rectangle

        private void InitRectangleEditor()
        {
            State.TempMode = State.Mode;
            State.Mode = SheetMode.Edit;

            try
            {
                var rectangle = State.SelectedBlock.Rectangles.FirstOrDefault();
                State.SelectedType = ItemType.Rectangle;
                State.SelectedElement = rectangle;
                InitFrameworkElementEditor();
            }
            catch (Exception ex)
            {
                Debug.Print(ex.Message);
                Debug.Print(ex.StackTrace);
            }
        }

        #endregion

        #region Edit Ellipse

        private void InitEllipseEditor()
        {
            State.TempMode = State.Mode;
            State.Mode = SheetMode.Edit;

            try
            {
                var ellipse = State.SelectedBlock.Ellipses.FirstOrDefault();
                State.SelectedType = ItemType.Ellipse;
                State.SelectedElement = ellipse;
                InitFrameworkElementEditor();
            }
            catch (Exception ex)
            {
                Debug.Print(ex.Message);
                Debug.Print(ex.StackTrace);
            }
        }

        #endregion

        #region Edit Text

        private void InitTextEditor()
        {
            State.TempMode = State.Mode;
            State.Mode = SheetMode.Edit;

            try
            {
                var text = State.SelectedBlock.Texts.FirstOrDefault();
                State.SelectedType = ItemType.Text;
                State.SelectedElement = text;
                InitFrameworkElementEditor();
            }
            catch (Exception ex)
            {
                Debug.Print(ex.Message);
                Debug.Print(ex.StackTrace);
            }
        }

        #endregion

        #region Edit Image

        private void InitImageEditor()
        {
            State.TempMode = State.Mode;
            State.Mode = SheetMode.Edit;

            try
            {
                var image = State.SelectedBlock.Images.FirstOrDefault();
                State.SelectedType = ItemType.Image;
                State.SelectedElement = image;
                InitFrameworkElementEditor();
            }
            catch (Exception ex)
            {
                Debug.Print(ex.Message);
                Debug.Print(ex.StackTrace);
            }
        }

        #endregion

        #region Data Binding

        public bool BindDataToBlock(ImmutablePoint p, DataItem dataItem)
        {
            var temp = CreateTempBlock();
            _blockController.HitTest(State.ContentSheet, State.ContentBlock, p, State.Options.HitTestSize, temp, true, false);

            if (_blockController.HaveOnlyOneBlockSelected(temp))
            {
                State.HistoryController.Register("Bind Data");
                var block = temp.Blocks[0];
                var result = BindDataToBlock(block, dataItem);
                _blockController.Deselect(temp);

                if (result == true)
                {
                    _blockController.Select(block);
                    State.SelectedBlock.Blocks = new List<IBlock>();
                    State.SelectedBlock.Blocks.Add(block);
                }

                return true;
            }

            _blockController.Deselect(temp);
            return false;
        }

        public bool BindDataToBlock(IBlock block, DataItem dataItem)
        {
            if (block != null && block.Texts != null
                && dataItem != null && dataItem.Columns != null && dataItem.Data != null
                && block.Texts.Count == dataItem.Columns.Length - 1)
            {
                // assign block data id
                block.DataId = int.Parse(dataItem.Data[0]);

                // skip index column
                int i = 1;

                foreach (var text in block.Texts)
                {
                    var tb = WpfBlockHelper.GetTextBlock(text);
                    tb.Text = dataItem.Data[i];
                    i++;
                }

                return true;
            }

            return false;
        }

        public void TryToBindData(ImmutablePoint p, DataItem dataItem)
        {
            // first try binding to existing block
            bool firstTryResult = BindDataToBlock(p, dataItem);

            // if failed insert selected block from library and try again to bind
            if (!firstTryResult)
            {
                var blockItem = State.LibraryController.GetSelected();
                if (blockItem != null)
                {
                    var block = Insert(blockItem, p, false);
                    bool secondTryResult = BindDataToBlock(block, dataItem);
                    if (!secondTryResult)
                    {
                        // remove block if failed to bind
                        var temp = CreateTempBlock();
                        temp.Blocks.Add(block);
                        _blockController.RemoveSelected(State.ContentSheet, State.ContentBlock, temp);
                    }
                    else
                    {
                        _blockController.Select(block);
                        State.SelectedBlock.Blocks = new List<IBlock>();
                        State.SelectedBlock.Blocks.Add(block);
                    }
                }
            }
        }

        #endregion

        #region New Page

        public void NewPage()
        {
            State.HistoryController.Register("New");
            ResetPage();
            CreatePage();
            State.ZoomController.AutoFit();
        }

        #endregion

        #region Open Page

        public async Task OpenTextPage(string path)
        {
            var text = await _itemController.OpenText(path);
            if (text != null)
            {
                var page = await Task.Run(() => _itemSerializer.DeserializeContents(text));
                State.HistoryController.Register("Open Text");
                ResetPage();
                DeserializePage(page);
            }
        }

        public async Task OpenJsonPage(string path)
        {
            var text = await _itemController.OpenText(path);
            if (text != null)
            {
                var page = await Task.Run(() => _jsonSerializer.Deerialize<BlockItem>(text));
                State.HistoryController.Register("Open Json");
                ResetPage();
                DeserializePage(page);
            }
        }

        public async void OpenPage()
        {
            var dlg = _serviceLocator.GetInstance<IOpenFileDialog>();
            dlg.Filter = FileDialogSettings.PageFilter;
            dlg.FilterIndex = 1;
            dlg.FileName = "";

            if (dlg.ShowDialog() == true)
            {
                string path = dlg.FileName;
                switch (dlg.FilterIndex)
                {
                    case 1:
                        {
                            try
                            {
                                await OpenTextPage(path);
                            }
                            catch (Exception ex)
                            {
                                Debug.Print(ex.Message);
                                Debug.Print(ex.StackTrace);
                            }
                        }
                        break;
                    case 2:
                    case 3:
                        {
                            try
                            {
                                await OpenJsonPage(path);
                            }
                            catch (Exception ex)
                            {
                                Debug.Print(ex.Message);
                                Debug.Print(ex.StackTrace);
                            }
                        }
                        break;
                }
            }
        }

        #endregion

        #region Save Page

        public void SaveTextPage(string path)
        {
            var page = SerializePage();

            Task.Run(() =>
            {
                var text = _itemSerializer.SerializeContents(page);
                _itemController.SaveText(path, text);
            });
        }

        public void SaveJsonPage(string path)
        {
            var page = SerializePage();

            Task.Run(() =>
            {
                string text = _jsonSerializer.Serialize(page);
                _itemController.SaveText(path, text);
            });
        }

        public void SavePage()
        {
            var dlg = _serviceLocator.GetInstance<ISaveFileDialog>();
            dlg.Filter = FileDialogSettings.PageFilter;
            dlg.FilterIndex = 1;
            dlg.FileName = "sheet";

            if (dlg.ShowDialog() == true)
            {
                string path = dlg.FileName;
                switch (dlg.FilterIndex)
                {
                    case 1:
                        {
                            try
                            {
                                SaveTextPage(path);
                            }
                            catch (Exception ex)
                            {
                                Debug.Print(ex.Message);
                                Debug.Print(ex.StackTrace);
                            }
                        }
                        break;
                    case 2:
                    case 3:
                        {
                            try
                            {
                                SaveJsonPage(path);
                            }
                            catch (Exception ex)
                            {
                                Debug.Print(ex.Message);
                                Debug.Print(ex.StackTrace);
                            }
                        }
                        break;
                }
            }
        }

        #endregion

        #region Export Page

        private void ExportToPdf(IEnumerable<BlockItem> blocks, string fileName)
        {
            var pages = blocks.Select(content => CreatePage(content, true, false)).ToList();

            Task.Run(() =>
            {
                var writer = new BlockPdfWriter();
                writer.Create(fileName, State.Options.PageWidth, State.Options.PageHeight, pages);
                Process.Start(fileName);
            });
        }

        private void ExportToDxf(IEnumerable<BlockItem> blocks, string fileName)
        {
            var pages = blocks.Select(block => CreatePage(block, true, false)).ToList();

            Task.Run(() =>
            {
                var writer = new BlockDxfWriter();

                if (blocks.Count() > 1)
                {
                    string path = System.IO.Path.GetDirectoryName(fileName);
                    string name = System.IO.Path.GetFileNameWithoutExtension(fileName);
                    string extension = System.IO.Path.GetExtension(fileName);

                    int counter = 0;
                    foreach (var page in pages)
                    {
                        string fileNameWithCounter = System.IO.Path.Combine(path, string.Concat(name, '-', counter.ToString("000"), extension));
                        writer.Create(fileNameWithCounter, State.Options.PageWidth, State.Options.PageHeight, page);
                        counter++;
                    }
                }
                else
                {
                    var page = pages.FirstOrDefault();
                    if (page != null)
                    {
                        writer.Create(fileName, State.Options.PageWidth, State.Options.PageHeight, page);
                    }
                }
            });
        }

        public void Export(IEnumerable<BlockItem> blocks)
        {
            var dlg = _serviceLocator.GetInstance<ISaveFileDialog>();
            dlg.Filter = FileDialogSettings.ExportFilter;
            dlg.FilterIndex = 1;
            dlg.FileName = "sheet";

            if (dlg.ShowDialog() == true)
            {
                string path = dlg.FileName;
                switch (dlg.FilterIndex)
                {
                    case 1:
                    case 3:
                    default:
                        {
                            try
                            {
                                ExportToPdf(blocks, path);
                            }
                            catch (Exception ex)
                            {
                                Debug.Print(ex.Message);
                                Debug.Print(ex.StackTrace);
                            }
                        }
                        break;
                    case 2:
                        {
                            try
                            {
                                ExportToDxf(blocks, path);
                            }
                            catch (Exception ex)
                            {
                                Debug.Print(ex.Message);
                                Debug.Print(ex.StackTrace);
                            }
                        }
                        break;
                }
            }
        }

        public void Export(SolutionEntry solution)
        {
            var texts = solution.Documents.SelectMany(document => document.Pages).Select(page => page.Content);
            ExportPages(texts);
        }

        public void ExportPage()
        {
            var block = _blockSerializer.SerializerAndSetId(State.ContentBlock, -1, State.ContentBlock.X, State.ContentBlock.Y, State.ContentBlock.Width, State.ContentBlock.Height, State.ContentBlock.DataId, "CONTENT");
            var blocks = ToSingle(block);
            Export(blocks);
        }

        #endregion

        #region Library

        public void Insert(ImmutablePoint p)
        {
            if (State.LibraryController != null)
            {
                var blockItem = State.LibraryController.GetSelected() as BlockItem;
                Insert(blockItem, p, true);
            }
        }

        public IBlock Insert(BlockItem blockItem, ImmutablePoint p, bool select)
        {
            _blockController.Deselect(State.SelectedBlock);
            State.SelectedBlock = CreateSelectedBlock();

            double thickness = State.Options.LineThickness / State.ZoomController.Zoom;

            State.HistoryController.Register("Insert Block");

            var block = _blockSerializer.Deserialize(State.ContentSheet, State.ContentBlock, blockItem, thickness);

            if (select)
            {
                _blockController.Select(block);
                State.SelectedBlock.Blocks = new List<IBlock>();
                State.SelectedBlock.Blocks.Add(block);
            }

            _blockController.MoveDelta(_itemController.Snap(p.X, State.Options.SnapSize), _itemController.Snap(p.Y, State.Options.SnapSize), block);

            return block;
        }

        private async void LoadLibraryFromResource(string name)
        {
            var assembly = System.Reflection.Assembly.GetExecutingAssembly();
            if (assembly == null)
            {
                return;
            }

            using (var stream = assembly.GetManifestResourceStream(name))
            {
                using (var reader = new System.IO.StreamReader(stream))
                {
                    string text = await reader.ReadToEndAsync();
                    if (text != null)
                    {
                        InitLibrary(text);
                    }
                }
            }
        }

        public async Task LoadLibrary(string path)
        {
            var text = await _itemController.OpenText(path);
            if (text != null)
            {
                InitLibrary(text);
            }
        }

        private async void InitLibrary(string text)
        {
            if (State.LibraryController != null && text != null)
            {
                var block = await Task.Run(() => _itemSerializer.DeserializeContents(text));
                State.LibraryController.SetSource(block.Blocks);
            }
        }

        private void AddToLibrary(BlockItem blockItem)
        {
            if (State.LibraryController != null && blockItem != null)
            {
                var source = State.LibraryController.GetSource();
                var items = new List<BlockItem>(source);
                _itemController.ResetPosition(blockItem, State.Options.PageOriginX, State.Options.PageOriginY, State.Options.PageWidth, State.Options.PageHeight);
                items.Add(blockItem);
                State.LibraryController.SetSource(items);
            }
        }

        public async void LoadLibrary()
        {
            var dlg = _serviceLocator.GetInstance<IOpenFileDialog>();
            dlg.Filter = FileDialogSettings.LibraryFilter;
            dlg.FilterIndex = 1;
            dlg.FileName = "";

            if (dlg.ShowDialog() == true)
            {
                try
                {
                    await LoadLibrary(dlg.FileName);
                }
                catch (Exception ex)
                {
                    Debug.Print(ex.Message);
                    Debug.Print(ex.StackTrace);
                }
            }
        }

        #endregion

        #region Point

        public IPoint TryToFindPoint(ImmutablePoint p)
        {
            var temp = CreateTempBlock();
            _blockController.HitTest(State.ContentSheet, State.ContentBlock, p, State.Options.HitTestSize, temp, true, true);

            // check if one point is selected and ignore selected blocks
            if (temp.Points.Count == 1
                && temp.Lines.Count == 0
                && temp.Rectangles.Count == 0
                && temp.Ellipses.Count == 0
                && temp.Texts.Count == 0
                && temp.Images.Count == 0)
            {
                var xpoint = temp.Points[0];
                _blockController.Deselect(temp);
                return xpoint;
            }

            _blockController.Deselect(temp);
            return null;
        }

        #endregion

        #region Input

        public void LeftDown(InputArgs args)
        {
            // edit mode
            if (State.SelectedType != ItemType.None)
            {
                if (args.SourceType != ItemType.Thumb)
                {
                    _blockController.Deselect(State.SelectedBlock);
                    State.SelectedBlock = CreateSelectedBlock();
                    FinishEdit();
                }
                else
                {
                    return;
                }
            }

            // text editor
            if (State.Mode == SheetMode.None || State.Mode == SheetMode.TextEditor)
            {
                return;
            }

            // move mode
            if (!args.OnlyControl)
            {
                if (_blockController.HaveSelected(State.SelectedBlock) && CanInitMove(args.SheetPosition))
                {
                    InitMove(args.SheetPosition);
                    return;
                }

                _blockController.Deselect(State.SelectedBlock);
                State.SelectedBlock = CreateSelectedBlock();
            }

            bool resetSelected = args.OnlyControl && _blockController.HaveSelected(State.SelectedBlock) ? false : true;

            if (State.Mode == SheetMode.Selection)
            {
                _blockController.Deselect(State.ContentBlock);

                State.SelectedBlock = CreateSelectedBlock();
                bool result = _blockController.HitTest(State.ContentSheet, State.ContentBlock, new ImmutablePoint(args.SheetPosition.X, args.SheetPosition.Y), State.Options.HitTestSize, State.SelectedBlock, true, false);

                if ((args.OnlyControl || !_blockController.HaveSelected(State.SelectedBlock)) && !result)
                {
                    InitSelectionRect(args.SheetPosition);
                }
                else
                {
                    _blockController.Select(State.SelectedBlock);

                    // TODO: If control key is pressed then switch to move mode instead to edit mode
                    bool editModeEnabled = args.OnlyControl == true ? false : TryToEditSelected();
                    if (!editModeEnabled)
                    {
                        InitMove(args.SheetPosition);
                    }
                }
            }
            else if (State.Mode == SheetMode.Insert && !State.OverlaySheet.IsCaptured)
            {
                Insert(args.SheetPosition);
            }
            else if (State.Mode == SheetMode.Point && !State.OverlaySheet.IsCaptured)
            {
                InsertPoint(args.SheetPosition, true, true);
            }
            else if (State.Mode == SheetMode.Line && !State.OverlaySheet.IsCaptured)
            {
                // try to find point to connect line start
                IPoint start = TryToFindPoint(args.SheetPosition);

                // create start if Control key is pressed and start point has not been found
                if (args.OnlyControl && start == null)
                {
                    start = InsertPoint(args.SheetPosition, true, false);
                }

                _lineMode.Init(args.SheetPosition, start);
            }
            else if (State.Mode == SheetMode.Line && State.OverlaySheet.IsCaptured)
            {
                // try to find point to connect line end
                IPoint end = TryToFindPoint(args.SheetPosition);

                // create end point if Control key is pressed and end point has not been found
                if (args.OnlyControl && end == null)
                {
                    end = InsertPoint(args.SheetPosition, true, false);
                }

                _lineMode.Finish(end);
            }
            else if (State.Mode == SheetMode.Rectangle && !State.OverlaySheet.IsCaptured)
            {
                _rectangleMode.Init(args.SheetPosition);
            }
            else if (State.Mode == SheetMode.Rectangle && State.OverlaySheet.IsCaptured)
            {
                _rectangleMode.Finish();
            }
            else if (State.Mode == SheetMode.Ellipse && !State.OverlaySheet.IsCaptured)
            {
                _ellipseMode.Init(args.SheetPosition);
            }
            else if (State.Mode == SheetMode.Ellipse && State.OverlaySheet.IsCaptured)
            {
                _ellipseMode.Finish();
            }
            else if (State.Mode == SheetMode.Pan && State.OverlaySheet.IsCaptured)
            {
                FinishPan();
            }
            else if (State.Mode == SheetMode.Text && !State.OverlaySheet.IsCaptured)
            {
                CreateText(args.SheetPosition);
            }
            else if (State.Mode == SheetMode.Image && !State.OverlaySheet.IsCaptured)
            {
                Image(args.SheetPosition);
            }
        }

        public void LeftUp(InputArgs args)
        {
            if (State.Mode == SheetMode.Selection && State.OverlaySheet.IsCaptured)
            {
                FinishSelectionRect();
            }
            else if (State.Mode == SheetMode.Move && State.OverlaySheet.IsCaptured)
            {
                FinishMove();
            }
        }

        public void Move(InputArgs args)
        {
            if (State.Mode == SheetMode.Edit)
            {
                return;
            }

            // TIP: mouse over selection when holding Shift key
            if (args.OnlyShift && State.TempSelectionRect == null && !State.OverlaySheet.IsCaptured)
            {
                if (_blockController.HaveSelected(State.SelectedBlock))
                {
                    _blockController.Deselect(State.SelectedBlock);
                    State.SelectedBlock = CreateSelectedBlock();
                }

                _blockController.HitTest(State.ContentSheet, State.ContentBlock, args.SheetPosition, State.Options.HitTestSize, State.SelectedBlock, true, false);
            }

            if (State.Mode == SheetMode.Selection && State.OverlaySheet.IsCaptured)
            {
                MoveSelectionRect(args.SheetPosition);
            }
            else if (State.Mode == SheetMode.Line && State.OverlaySheet.IsCaptured)
            {
                _lineMode.Move(args.SheetPosition);
            }
            else if (State.Mode == SheetMode.Rectangle && State.OverlaySheet.IsCaptured)
            {
                _rectangleMode.Move(args.SheetPosition);
            }
            else if (State.Mode == SheetMode.Ellipse && State.OverlaySheet.IsCaptured)
            {
                _ellipseMode.Move(args.SheetPosition);
            }
            else if (State.Mode == SheetMode.Pan && State.OverlaySheet.IsCaptured)
            {
                Pan(args.RootPosition);
            }
            else if (State.Mode == SheetMode.Move && State.OverlaySheet.IsCaptured)
            {
                Move(args.SheetPosition);
            }
        }

        public void RightDown(InputArgs args)
        {
            if (State.Mode == SheetMode.None || State.Mode == SheetMode.TextEditor)
            {
                return;
            }

            // edit mode
            if (State.SelectedType != ItemType.None)
            {
                _blockController.Deselect(State.SelectedBlock);
                State.SelectedBlock = CreateSelectedBlock();
                FinishEdit();
                return;
            }

            // text editor
            if (State.Mode == SheetMode.Text && TryToEditText(args.SheetPosition))
            {
                args.Handled(true);
                return;
            }
            else
            {
                _blockController.Deselect(State.SelectedBlock);
                State.SelectedBlock = CreateSelectedBlock();

                if (State.Mode == SheetMode.Selection && State.OverlaySheet.IsCaptured)
                {
                    CancelSelectionRect();
                }
                else if (State.Mode == SheetMode.Line && State.OverlaySheet.IsCaptured)
                {
                    _lineMode.Cancel();
                }
                else if (State.Mode == SheetMode.Rectangle && State.OverlaySheet.IsCaptured)
                {
                    _rectangleMode.Cancel();
                }
                else if (State.Mode == SheetMode.Ellipse && State.OverlaySheet.IsCaptured)
                {
                    _ellipseMode.Cancel();
                }
                else if (!State.OverlaySheet.IsCaptured)
                {
                    InitPan(args.RootPosition);
                }
            }
        }

        public void RightUp(InputArgs args)
        {
            if (State.Mode == SheetMode.Pan && State.OverlaySheet.IsCaptured)
            {
                FinishPan();
            }
        }

        public void Wheel(int delta, ImmutablePoint position)
        {
            ZoomTo(delta, position);
        }

        public void Down(InputArgs args)
        {
            if (args.Button == InputButton.Middle && args.Clicks == 2)
            {
                // Mouse Middle Double-Click + Control key pressed to reset Pan and Zoom
                // Mouse Middle Double-Click to Auto Fit page to window size
                if (args.OnlyControl)
                {
                    State.ZoomController.ActualSize();
                }
                else
                {
                    State.ZoomController.AutoFit();
                }
            }
        }

        #endregion

        #region Page Frame & Grid

        private void CreatePage()
        {
            _pageFactory.CreateGrid(State.BackSheet, State.GridBlock, 330.0, 30.0, 600.0, 750.0, State.Options.GridSize, State.Options.GridThickness, ItemColors.LightGray);
            _pageFactory.CreateFrame(State.BackSheet, State.FrameBlock, State.Options.GridSize, State.Options.GridThickness, ItemColors.DarkGray);

            AdjustThickness(State.GridBlock, State.Options.GridThickness / GetZoom(State.ZoomController.ZoomIndex));
            AdjustThickness(State.FrameBlock, State.Options.FrameThickness / GetZoom(State.ZoomController.ZoomIndex));
        }

        private BlockItem CreateGridBlock(IBlock gridBlock, bool adjustThickness, bool adjustColor)
        {
            var grid = _blockSerializer.SerializerAndSetId(gridBlock, -1, 0.0, 0.0, 0.0, 0.0, -1, "GRID");

            // lines
            foreach (var lineItem in grid.Lines)
            {
                if (adjustThickness)
                {
                    lineItem.StrokeThickness = 0.013 * 72.0 / 2.54; // 0.13mm 
                }

                if (adjustColor)
                {
                    lineItem.Stroke = ItemColors.Black;
                }
            }

            return grid;
        }

        private BlockItem CreateFrameBlock(IBlock frameBlock, bool adjustThickness, bool adjustColor)
        {
            var frame = _blockSerializer.SerializerAndSetId(frameBlock, -1, 0.0, 0.0, 0.0, 0.0, -1, "FRAME");

            // texts
            foreach (var textItem in frame.Texts)
            {
                if (adjustColor)
                {
                    textItem.Foreground = ItemColors.Black;
                }
            }

            // lines
            foreach (var lineItem in frame.Lines)
            {
                if (adjustThickness)
                {
                    lineItem.StrokeThickness = 0.018 * 72.0 / 2.54; // 0.18mm 
                }

                if (adjustColor)
                {
                    lineItem.Stroke = ItemColors.Black;
                }
            }

            return frame;
        }

        private BlockItem CreatePage(BlockItem content, bool enableFrame, bool enableGrid)
        {
            var page = new BlockItem(-1, 0.0, 0.0, 0.0, 0.0, -1, "PAGE", new ItemColor() { Alpha = 0, Red = 0, Green = 0, Blue = 0 });

            if (enableGrid)
            {
                var grid = CreateGridBlock(State.GridBlock, true, false);
                page.Blocks.Add(grid);
            }

            if (enableFrame)
            {
                var frame = CreateFrameBlock(State.FrameBlock, true, true);
                page.Blocks.Add(frame);
            }

            page.Blocks.Add(content);

            return page;
        }

        #endregion

        #region Plugins

        private ISelectedBlockPlugin _invertedLineStartPlugin;
        private ISelectedBlockPlugin _invertedLineEndPlugin;

        private void CreatePlugins()
        {
            _invertedLineStartPlugin = new InvertLineStartPlugin(_serviceLocator);
            _invertedLineEndPlugin = new InvertLineEndPlugin(_serviceLocator);
        }

        private void ProcessPlugin(ISelectedBlockPlugin plugin)
        {
            if (plugin.CanProcess(State.ContentSheet, State.ContentBlock, State.SelectedBlock, State.Options))
            {
                var copy = _blockFactory.CreateBlock(State.SelectedBlock.Id, State.SelectedBlock.X, State.SelectedBlock.Y, State.SelectedBlock.Width, State.SelectedBlock.Height, State.SelectedBlock.DataId, State.SelectedBlock.Name, null);
                _blockController.ShallowCopy(State.SelectedBlock, copy);

                _blockController.Deselect(State.SelectedBlock);
                State.SelectedBlock = CreateSelectedBlock();

                FinishEdit();
                State.HistoryController.Register(plugin.Name);

                plugin.Process(State.ContentSheet, State.ContentBlock, copy, State.Options);

                State.SelectedBlock = copy;
            }
        }

        public void InvertSelectedLineStart()
        {
            ProcessPlugin(_invertedLineStartPlugin);
        }

        public void InvertSelectedLineEnd()
        {
            ProcessPlugin(_invertedLineEndPlugin);
        }

        #endregion
    }

    public class SheetHistoryController : IHistoryController
    {
        #region IoC

        private readonly ISheetController _sheetController;
        private readonly IItemSerializer _itemSerializer;

        public SheetHistoryController(ISheetController sheetController, IItemSerializer itemSerializer)
        {
            this._sheetController = sheetController;
            this._itemSerializer = itemSerializer;
        }

        #endregion

        #region Fields

        private Stack<ChangeItem> undos = new Stack<ChangeItem>();
        private Stack<ChangeItem> redos = new Stack<ChangeItem>();

        #endregion

        #region Factory

        private async Task<ChangeItem> CreateChange(string message)
        {
            var block = _sheetController.SerializePage();
            var text = await Task.Run(() => _itemSerializer.SerializeContents(block));
            var change = new ChangeItem()
            {
                Message = message,
                Model = text
            };
            return change;
        }

        #endregion

        #region IHistoryController

        public async void Register(string message)
        {
            var change = await CreateChange(message);
            undos.Push(change);
            redos.Clear();
        }

        public void Reset()
        {
            undos.Clear();
            redos.Clear();
        }

        public async void Undo()
        {
            if (undos.Count > 0)
            {
                try
                {
                    var change = await CreateChange("Redo");
                    redos.Push(change);
                    var undo = undos.Pop();
                    var block = await Task.Run(() => _itemSerializer.DeserializeContents(undo.Model));
                    _sheetController.ResetPage();
                    _sheetController.DeserializePage(block);
                }
                catch (Exception ex)
                {
                    Debug.Print(ex.Message);
                    Debug.Print(ex.StackTrace);
                }
            }
        }

        public async void Redo()
        {
            if (redos.Count > 0)
            {
                try
                {
                    var change = await CreateChange("Undo");
                    undos.Push(change);
                    var redo = redos.Pop();
                    var block = await Task.Run(() => _itemSerializer.DeserializeContents(redo.Model));
                    _sheetController.ResetPage();
                    _sheetController.DeserializePage(block);
                }
                catch (Exception ex)
                {
                    Debug.Print(ex.Message);
                    Debug.Print(ex.StackTrace);
                }
            }
        }

        #endregion
    }

    public class SheetPageFactory : IPageFactory
    {
        #region IoC

        private readonly IServiceLocator _serviceLocator;
        private readonly IBlockFactory _blockFactory;

        public SheetPageFactory(IServiceLocator serviceLocator)
        {
            this._serviceLocator = serviceLocator;
            this._blockFactory = serviceLocator.GetInstance<IBlockFactory>();
        }

        #endregion

        #region Create

        public void CreateLine(ISheet sheet, IList<ILine> lines, double thickness, double x1, double y1, double x2, double y2, ItemColor stroke)
        {
            var line = _blockFactory.CreateLine(thickness, x1, y1, x2, y2, stroke);

            if (lines != null)
            {
                lines.Add(line);
            }

            if (sheet != null)
            {
                sheet.Add(line);
            }
        }

        public void CreateText(ISheet sheet, IList<IText> texts, string content, double x, double y, double width, double height, int halign, int valign, double size, ItemColor foreground)
        {
            var text = _blockFactory.CreateText(content, x, y, width, height, halign, valign, size, ItemColors.Transparent, foreground);

            if (texts != null)
            {
                texts.Add(text);
            }

            if (sheet != null)
            {
                sheet.Add(text);
            }
        }

        public void CreateFrame(ISheet sheet, IBlock block, double size, double thickness, ItemColor stroke)
        {
            double padding = 6.0;
            double width = 1260.0;
            double height = 891.0;

            double startX = padding;
            double startY = padding;

            double rowsStart = 60;
            double rowsEnd = 780.0;

            double tableStartX = startX;
            double tableStartY = rowsEnd + 25.0;

            bool frameShowBorder = true;
            bool frameShowRows = true;
            bool frameShowTable = true;

            double row0 = 0.0;
            double row1 = 20.0;
            double row2 = 40.0;
            double row3 = 60.0;
            double row4 = 80.0;

            bool tableShowRevisions = true;
            bool tableShowLogos = true;
            bool tableShowInfo = true;

            if (frameShowRows)
            {
                // frame headers
                CreateText(sheet, block.Texts, "I N P U T S", padding, padding, 330.0 - padding, 30.0 - padding, (int)XHorizontalAlignment.Center, (int)XVerticalAlignment.Center, 16.0, stroke);
                CreateText(sheet, block.Texts, "F U N C T I O N", 330.0, padding, 600.0, 30.0 - padding, (int)XHorizontalAlignment.Center, (int)XVerticalAlignment.Center, 16.0, stroke);
                CreateText(sheet, block.Texts, "O U T P U T S", 930.0, padding, 330.0 - padding, 30.0 - padding, (int)XHorizontalAlignment.Center, (int)XVerticalAlignment.Center, 16.0, stroke);

                // frame left rows
                int leftRowNumber = 1;
                for (double y = rowsStart; y < rowsEnd; y += size)
                {
                    CreateLine(sheet, block.Lines, thickness, startX, y, 330.0, y, stroke);
                    CreateText(sheet, block.Texts, leftRowNumber.ToString("00"), startX, y, 30.0 - padding, size, (int)XHorizontalAlignment.Center, (int)XVerticalAlignment.Center, 14.0, stroke);
                    leftRowNumber++;
                }

                // frame right rows
                int rightRowNumber = 1;
                for (double y = rowsStart; y < rowsEnd; y += size)
                {
                    CreateLine(sheet, block.Lines, thickness, 930.0, y, width - padding, y, stroke);
                    CreateText(sheet, block.Texts, rightRowNumber.ToString("00"), width - 30.0, y, 30.0 - padding, size, (int)XHorizontalAlignment.Center, (int)XVerticalAlignment.Center, 14.0, stroke);
                    rightRowNumber++;
                }

                // frame columns
                double[] columnWidth = { 30.0, 210.0, 90.0, 600.0, 210.0, 90.0 };
                double[] columnX = { 30.0, 30.0, startY, startY, 30.0, 30.0 };
                double[] columnY = { rowsEnd, rowsEnd, rowsEnd, rowsEnd, rowsEnd, rowsEnd };

                double start = 0.0;
                for (int i = 0; i < columnWidth.Length; i++)
                {
                    start += columnWidth[i];
                    CreateLine(sheet, block.Lines, thickness, start, columnX[i], start, columnY[i], stroke);
                }

                // frame header
                CreateLine(sheet, block.Lines, thickness, startX, 30.0, width - padding, 30.0, stroke);

                // frame footer
                CreateLine(sheet, block.Lines, thickness, startX, rowsEnd, width - padding, rowsEnd, stroke);
            }

            if (frameShowTable)
            {
                // table header
                CreateLine(sheet, block.Lines, thickness, tableStartX + 0, tableStartY + row0, tableStartX + 1248, tableStartY + row0, stroke);

                // table revisions
                if (tableShowRevisions)
                {
                    CreateLine(sheet, block.Lines, thickness, tableStartX + 24, tableStartY + row0, tableStartX + 24, tableStartY + row4, stroke);
                    CreateLine(sheet, block.Lines, thickness, tableStartX + 75, tableStartY + row0, tableStartX + 75, tableStartY + row4, stroke);
                    CreateLine(sheet, block.Lines, thickness, tableStartX + 0, tableStartY + row1, tableStartX + 175, tableStartY + row1, stroke);
                    CreateLine(sheet, block.Lines, thickness, tableStartX + 0, tableStartY + row2, tableStartX + 175, tableStartY + row2, stroke);
                    CreateLine(sheet, block.Lines, thickness, tableStartX + 0, tableStartY + row3, tableStartX + 175, tableStartY + row3, stroke);
                }

                // table logos
                if (tableShowLogos)
                {
                    CreateLine(sheet, block.Lines, thickness, tableStartX + 175, tableStartY + row0, tableStartX + 175, tableStartY + row4, stroke);
                    CreateLine(sheet, block.Lines, thickness, tableStartX + 290, tableStartY + row0, tableStartX + 290, tableStartY + row4, stroke);
                    CreateLine(sheet, block.Lines, thickness, tableStartX + 405, tableStartY + row0, tableStartX + 405, tableStartY + row4, stroke);
                }

                // table info
                if (tableShowInfo)
                {
                    CreateLine(sheet, block.Lines, thickness, tableStartX + 405, tableStartY + row1, tableStartX + 1248, tableStartY + row1, stroke);
                    CreateLine(sheet, block.Lines, thickness, tableStartX + 405, tableStartY + row2, tableStartX + 695, tableStartY + row2, stroke);
                    CreateLine(sheet, block.Lines, thickness, tableStartX + 965, tableStartY + row2, tableStartX + 1248, tableStartY + row2, stroke);
                    CreateLine(sheet, block.Lines, thickness, tableStartX + 405, tableStartY + row3, tableStartX + 695, tableStartY + row3, stroke);
                    CreateLine(sheet, block.Lines, thickness, tableStartX + 965, tableStartY + row3, tableStartX + 1248, tableStartY + row3, stroke);

                    CreateLine(sheet, block.Lines, thickness, tableStartX + 465, tableStartY + row0, tableStartX + 465, tableStartY + row4, stroke);
                    CreateLine(sheet, block.Lines, thickness, tableStartX + 595, tableStartY + row0, tableStartX + 595, tableStartY + row4, stroke);
                    CreateLine(sheet, block.Lines, thickness, tableStartX + 640, tableStartY + row0, tableStartX + 640, tableStartY + row4, stroke);
                    CreateLine(sheet, block.Lines, thickness, tableStartX + 695, tableStartY + row0, tableStartX + 695, tableStartY + row4, stroke);
                    CreateLine(sheet, block.Lines, thickness, tableStartX + 965, tableStartY + row0, tableStartX + 965, tableStartY + row4, stroke);

                    CreateLine(sheet, block.Lines, thickness, tableStartX + 1005, tableStartY + row0, tableStartX + 1005, tableStartY + row4, stroke);
                    CreateLine(sheet, block.Lines, thickness, tableStartX + 1045, tableStartY + row0, tableStartX + 1045, tableStartY + row4, stroke);
                    CreateLine(sheet, block.Lines, thickness, tableStartX + 1100, tableStartY + row0, tableStartX + 1100, tableStartY + row4, stroke);
                }
            }

            if (frameShowBorder)
            {
                // frame border
                CreateLine(sheet, block.Lines, thickness, startX, startY, width - padding, startY, stroke);
                CreateLine(sheet, block.Lines, thickness, startX, height - padding, width - padding, height - padding, stroke);
                CreateLine(sheet, block.Lines, thickness, startX, startY, startX, height - padding, stroke);
                CreateLine(sheet, block.Lines, thickness, width - padding, startY, width - padding, height - padding, stroke);
            }
        }

        public void CreateGrid(ISheet sheet, IBlock block, double startX, double startY, double width, double height, double size, double thickness, ItemColor stroke)
        {
            for (double y = startY + size; y < height + startY; y += size)
            {
                CreateLine(sheet, block.Lines, thickness, startX, y, width + startX, y, stroke);
            }

            for (double x = startX + size; x < startX + width; x += size)
            {
                CreateLine(sheet, block.Lines, thickness, x, startY, x, height + startY, stroke);
            }
        }

        public IRectangle CreateSelectionRectangle(double thickness, double x, double y, double width, double height)
        {
            var stroke = new ItemColor() { Alpha = 0x7A, Red = 0x00, Green = 0x00, Blue = 0xFF };
            var fill = new ItemColor() { Alpha = 0x3A, Red = 0x00, Green = 0x00, Blue = 0xFF };
            var xrect = _blockFactory.CreateRectangle(thickness, x, y, width, height, true, stroke, fill);
            return xrect;
        }

        #endregion
    }
}
