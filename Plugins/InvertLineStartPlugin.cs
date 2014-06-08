﻿using Sheet.Block;
using Sheet.Block.Core;
using Sheet.Item;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sheet.Plugins
{
    public class InvertLineStartPlugin : ISelectedBlockPlugin
    {
        #region IoC

        private readonly IServiceLocator _serviceLocator;
        private readonly IBlockController _blockController;
        private readonly IBlockFactory _blockFactory;
        private readonly IBlockHelper _blockHelper;

        public InvertLineStartPlugin(IServiceLocator serviceLocator)
        {
            _serviceLocator = serviceLocator;
            _blockController = serviceLocator.GetInstance<IBlockController>();
            _blockFactory = serviceLocator.GetInstance<IBlockFactory>();
            _blockHelper = serviceLocator.GetInstance<IBlockHelper>();
        }

        #endregion

        #region ISelectedBlockPlugin

        public string Name
        {
            get { return "Invert Line Start"; }
        }

        public bool CanProcess(ISheet contentSheet, IBlock contentBlock, IBlock selectedBlock, SheetOptions options)
        {
            return _blockController.HaveSelected(selectedBlock) && selectedBlock.Lines != null && selectedBlock.Lines.Count > 0;
        }

        public void Process(ISheet contentSheet, IBlock contentBlock, IBlock selectedBlock, SheetOptions options)
        {
            InvertSelectedLineStart(contentSheet, contentBlock, selectedBlock, options);
        }

        #endregion

        #region Plugin Code

        private double invertedEllipseWidth = 10.0;
        private double invertedEllipseHeight = 10.0;

        private void AddInvertedLineEllipse(ISheet contentSheet, IBlock contentBlock, IBlock selectedBlock, SheetOptions options, double x, double y, double width, double height)
        {
            // create ellipse
            var ellipse = _blockFactory.CreateEllipse(options.LineThickness / options.Zoom, x, y, width, height, false, ItemColors.Black, ItemColors.Transparent);
            contentBlock.Ellipses.Add(ellipse);
            contentSheet.Add(ellipse);

            // select ellipse
            _blockController.Select(ellipse);
            if (selectedBlock.Ellipses == null)
            {
                selectedBlock.Ellipses = new List<IEllipse>();
            }
            selectedBlock.Ellipses.Add(ellipse);
        }

        private void InvertSelectedLineStart(ISheet contentSheet, IBlock contentBlock, IBlock selectedBlock, SheetOptions options)
        {
            // add for horizontal or vertical line start ellipse and shorten line
            if (_blockController.HaveSelected(selectedBlock) && selectedBlock.Lines != null && selectedBlock.Lines.Count > 0)
            {
                var block = _blockController.ShallowCopy(selectedBlock);

                _blockController.DeselectContent(selectedBlock);

                foreach (var line in block.Lines)
                {
                    double x1 = _blockHelper.GetX1(line);
                    double y1 = _blockHelper.GetY1(line);
                    double x2 = _blockHelper.GetX2(line);
                    double y2 = _blockHelper.GetY2(line);
                    bool sameX = Math.Round(x1, 1) == Math.Round(x2, 1);
                    bool sameY = Math.Round(y1, 1) == Math.Round(y2, 1);

                    // vertical line
                    if (sameX && !sameY)
                    {
                        // X1, Y1 is start position
                        if (y1 < y2)
                        {
                            AddInvertedLineEllipse(contentSheet, contentBlock, selectedBlock, options, x1 - invertedEllipseWidth / 2.0, y1, invertedEllipseWidth, invertedEllipseHeight);
                            _blockHelper.SetY1(line, y1 + invertedEllipseHeight);
                        }
                        // X2, Y2 is start position
                        else
                        {
                            AddInvertedLineEllipse(contentSheet, contentBlock, selectedBlock, options, x2 - invertedEllipseWidth / 2.0, y2, invertedEllipseWidth, invertedEllipseHeight);
                            _blockHelper.SetY2(line, y2 + invertedEllipseHeight);
                        }
                    }
                    // horizontal line
                    else if (!sameX && sameY)
                    {
                        // X1, Y1 is start position
                        if (x1 < x2)
                        {
                            AddInvertedLineEllipse(contentSheet, contentBlock, selectedBlock, options, x1, y1 - invertedEllipseHeight / 2.0, invertedEllipseWidth, invertedEllipseHeight);
                            _blockHelper.SetX1(line, x1 + invertedEllipseWidth);
                        }
                        // X2, Y2 is start position
                        else
                        {
                            AddInvertedLineEllipse(contentSheet, contentBlock, selectedBlock, options, x2, y2 - invertedEllipseHeight / 2.0, invertedEllipseWidth, invertedEllipseHeight);
                            _blockHelper.SetX2(line, x2 + invertedEllipseWidth);
                        }
                    }

                    // select line
                    _blockController.Select(line);
                    if (selectedBlock.Lines == null)
                    {
                        selectedBlock.Lines = new List<ILine>();
                    }
                    selectedBlock.Lines.Add(line);
                }
            }
        }

        #endregion
    }
}
