﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sheet
{
    #region Item Model

    public enum ItemType
    {
        None,
        Line,
        Rectangle,
        Ellipse,
        Text,
        Image
    }

    public abstract class Item
    {
        public int Id { get; set; }
    }

    public class ItemColor
    {
        public int Alpha { get; set; }
        public int Red { get; set; }
        public int Green { get; set; }
        public int Blue { get; set; }
    }

    public class LineItem : Item
    {
        public double X1 { get; set; }
        public double Y1 { get; set; }
        public double X2 { get; set; }
        public double Y2 { get; set; }
        public ItemColor Stroke { get; set; }
        public double StrokeThickness { get; set; }
    }

    public class RectangleItem : Item
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }
        public bool IsFilled { get; set; }
        public ItemColor Stroke { get; set; }
        public ItemColor Fill { get; set; }
        public double StrokeThickness { get; set; }
    }

    public class EllipseItem : Item
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }
        public bool IsFilled { get; set; }
        public ItemColor Stroke { get; set; }
        public ItemColor Fill { get; set; }
        public double StrokeThickness { get; set; }
    }

    public class TextItem : Item
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }
        public int HAlign { get; set; }
        public int VAlign { get; set; }
        public double Size { get; set; }
        public string Text { get; set; }
        public ItemColor Foreground { get; set; }
        public ItemColor Backgroud { get; set; }
    }

    public class ImageItem : Item
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }
        public byte[] Data { get; set; }
    }

    public class BlockItem : Item
    {
        public double X { get; set; }
        public double Y { get; set; }
        public string Name { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }
        public ItemColor Backgroud { get; set; }
        public int DataId { get; set; }
        public List<LineItem> Lines { get; set; }
        public List<RectangleItem> Rectangles { get; set; }
        public List<EllipseItem> Ellipses { get; set; }
        public List<TextItem> Texts { get; set; }
        public List<ImageItem> Images { get; set; }
        public List<BlockItem> Blocks { get; set; }
        public void Init(int id, double x, double y, int dataId, string name)
        {
            X = x;
            Y = y;
            Id = id;
            DataId = dataId;
            Name = name;
            Width = 0.0;
            Height = 0.0;
            Backgroud = new ItemColor() { Alpha = 0, Red = 0, Green = 0, Blue = 0 };
            Lines = new List<LineItem>();
            Rectangles = new List<RectangleItem>();
            Ellipses = new List<EllipseItem>();
            Texts = new List<TextItem>();
            Images = new List<ImageItem>();
            Blocks = new List<BlockItem>();
        }
    }

    public interface ILibrary
    {
        BlockItem GetSelected();
        void SetSelected(BlockItem block);
        IEnumerable<BlockItem> GetSource();
        void SetSource(IEnumerable<BlockItem> source);
    }

    public class DataItem
    {
        public string[] Columns { get; set; }
        public string[] Data { get; set; }
    }

    public interface IDatabase
    {
        string[] Get(int index);
        bool Update(int index, string[] item);
        int Add(string[] item);
    }

    public interface ITextController
    {
        void Set(Action<string> ok, Action cancel, string title, string label, string text);
    }

    public interface IBlockController
    {
        BlockItem Serialize();
        void Insert(BlockItem block);
        void Reset();
    }

    #endregion

    #region Item Serializer

    public class ItemSerializeOptions
    {
        #region Properties

        public string LineSeparator { get; set; }
        public string ModelSeparator { get; set; }
        public char[] LineSeparators { get; set; }
        public char[] ModelSeparators { get; set; }
        public char[] WhiteSpace { get; set; }
        public string IndentWhiteSpace { get; set; }
        public static ItemSerializeOptions Default
        {
            get
            {
                return new ItemSerializeOptions()
                {
                    LineSeparator = "\r\n",
                    ModelSeparator = ";",
                    LineSeparators = new char[] { '\r', '\n' },
                    ModelSeparators = new char[] { ';' },
                    WhiteSpace = new char[] { ' ', '\t' },
                    IndentWhiteSpace = "    "
                };
            }
        }

        #endregion
    }

    public static class ItemSerializer
    {
        #region Serialize

        public static void Serialize(StringBuilder sb, ItemColor color, ItemSerializeOptions options)
        {
            sb.Append(color.Alpha);
            sb.Append(options.ModelSeparator);
            sb.Append(color.Red);
            sb.Append(options.ModelSeparator);
            sb.Append(color.Green);
            sb.Append(options.ModelSeparator);
            sb.Append(color.Blue);
        }

        public static void Serialize(StringBuilder sb, LineItem line, string indent, ItemSerializeOptions options)
        {
            sb.Append(indent);
            sb.Append("LINE");
            sb.Append(options.ModelSeparator);
            sb.Append(line.Id);
            sb.Append(options.ModelSeparator);
            sb.Append(line.X1);
            sb.Append(options.ModelSeparator);
            sb.Append(line.Y1);
            sb.Append(options.ModelSeparator);
            sb.Append(line.X2);
            sb.Append(options.ModelSeparator);
            sb.Append(line.Y2);
            sb.Append(options.ModelSeparator);
            Serialize(sb, line.Stroke, options);
            sb.Append(options.LineSeparator);
        }

        public static void Serialize(StringBuilder sb, RectangleItem rectangle, string indent, ItemSerializeOptions options)
        {
            sb.Append(indent);
            sb.Append("RECTANGLE");
            sb.Append(options.ModelSeparator);
            sb.Append(rectangle.Id);
            sb.Append(options.ModelSeparator);
            sb.Append(rectangle.X);
            sb.Append(options.ModelSeparator);
            sb.Append(rectangle.Y);
            sb.Append(options.ModelSeparator);
            sb.Append(rectangle.Width);
            sb.Append(options.ModelSeparator);
            sb.Append(rectangle.Height);
            sb.Append(options.ModelSeparator);
            sb.Append(rectangle.IsFilled);
            sb.Append(options.ModelSeparator);
            Serialize(sb, rectangle.Stroke, options);
            sb.Append(options.ModelSeparator);
            Serialize(sb, rectangle.Fill, options);
            sb.Append(options.LineSeparator);
        }

        public static void Serialize(StringBuilder sb, EllipseItem ellipse, string indent, ItemSerializeOptions options)
        {
            sb.Append(indent);
            sb.Append("ELLIPSE");
            sb.Append(options.ModelSeparator);
            sb.Append(ellipse.Id);
            sb.Append(options.ModelSeparator);
            sb.Append(ellipse.X);
            sb.Append(options.ModelSeparator);
            sb.Append(ellipse.Y);
            sb.Append(options.ModelSeparator);
            sb.Append(ellipse.Width);
            sb.Append(options.ModelSeparator);
            sb.Append(ellipse.Height);
            sb.Append(options.ModelSeparator);
            sb.Append(ellipse.IsFilled);
            sb.Append(options.ModelSeparator);
            Serialize(sb, ellipse.Stroke, options);
            sb.Append(options.ModelSeparator);
            Serialize(sb, ellipse.Fill, options);
            sb.Append(options.LineSeparator);
        }

        public static void Serialize(StringBuilder sb, TextItem text, string indent, ItemSerializeOptions options)
        {
            sb.Append(indent);
            sb.Append("TEXT");
            sb.Append(options.ModelSeparator);
            sb.Append(text.Id);
            sb.Append(options.ModelSeparator);
            sb.Append(text.X);
            sb.Append(options.ModelSeparator);
            sb.Append(text.Y);
            sb.Append(options.ModelSeparator);
            sb.Append(text.Width);
            sb.Append(options.ModelSeparator);
            sb.Append(text.Height);
            sb.Append(options.ModelSeparator);
            sb.Append(text.HAlign);
            sb.Append(options.ModelSeparator);
            sb.Append(text.VAlign);
            sb.Append(options.ModelSeparator);
            sb.Append(text.Size);
            sb.Append(options.ModelSeparator);
            Serialize(sb, text.Foreground, options);
            sb.Append(options.ModelSeparator);
            Serialize(sb, text.Backgroud, options);
            sb.Append(options.ModelSeparator);
            sb.Append(text.Text);
            sb.Append(options.LineSeparator);
        }

        public static void Serialize(StringBuilder sb, ImageItem image, string indent, ItemSerializeOptions options)
        {
            sb.Append(indent);
            sb.Append("IMAGE");
            sb.Append(options.ModelSeparator);
            sb.Append(image.Id);
            sb.Append(options.ModelSeparator);
            sb.Append(image.X);
            sb.Append(options.ModelSeparator);
            sb.Append(image.Y);
            sb.Append(options.ModelSeparator);
            sb.Append(image.Width);
            sb.Append(options.ModelSeparator);
            sb.Append(image.Height);
            sb.Append(options.ModelSeparator);
            sb.Append(Base64.ToBase64(image.Data));
            sb.Append(options.LineSeparator);
        }

        public static void Serialize(StringBuilder sb, BlockItem block, string indent, ItemSerializeOptions options)
        {
            sb.Append(indent);
            sb.Append("BLOCK");
            sb.Append(options.ModelSeparator);
            sb.Append(block.Id);
            sb.Append(options.ModelSeparator);
            sb.Append(block.X);
            sb.Append(options.ModelSeparator);
            sb.Append(block.Y);
            sb.Append(options.ModelSeparator);
            sb.Append(block.Name);
            sb.Append(options.ModelSeparator);
            sb.Append(block.Width);
            sb.Append(options.ModelSeparator);
            sb.Append(block.Height);
            sb.Append(options.ModelSeparator);
            Serialize(sb, block.Backgroud, options);
            sb.Append(options.ModelSeparator);
            sb.Append(block.DataId);
            sb.Append(options.LineSeparator);

            Serialize(sb, block.Lines, indent + options.IndentWhiteSpace, options);
            Serialize(sb, block.Rectangles, indent + options.IndentWhiteSpace, options);
            Serialize(sb, block.Ellipses, indent + options.IndentWhiteSpace, options);
            Serialize(sb, block.Texts, indent + options.IndentWhiteSpace, options);
            Serialize(sb, block.Images, indent + options.IndentWhiteSpace, options);
            Serialize(sb, block.Blocks, indent + options.IndentWhiteSpace, options);

            sb.Append(indent);
            sb.Append("END");
            sb.Append(options.LineSeparator);
        }

        public static void Serialize(StringBuilder sb, IEnumerable<LineItem> lines, string indent, ItemSerializeOptions options)
        {
            foreach (var line in lines)
            {
                Serialize(sb, line, indent, options);
            }
        }

        public static void Serialize(StringBuilder sb, IEnumerable<RectangleItem> rectangles, string indent, ItemSerializeOptions options)
        {
            foreach (var rectangle in rectangles)
            {
                Serialize(sb, rectangle, indent, options);
            }
        }

        public static void Serialize(StringBuilder sb, IEnumerable<EllipseItem> ellipses, string indent, ItemSerializeOptions options)
        {
            foreach (var ellipse in ellipses)
            {
                Serialize(sb, ellipse, indent, options);
            }
        }

        public static void Serialize(StringBuilder sb, IEnumerable<TextItem> texts, string indent, ItemSerializeOptions options)
        {
            foreach (var text in texts)
            {
                Serialize(sb, text, indent, options);
            }
        }

        public static void Serialize(StringBuilder sb, IEnumerable<ImageItem> images, string indent, ItemSerializeOptions options)
        {
            foreach (var image in images)
            {
                Serialize(sb, image, indent, options);
            }
        }

        public static void Serialize(StringBuilder sb, IEnumerable<BlockItem> blocks, string indent, ItemSerializeOptions options)
        {
            foreach (var block in blocks)
            {
                Serialize(sb, block, indent, options);
            }
        }

        public static string SerializeContents(BlockItem block, ItemSerializeOptions options)
        {
            var sb = new StringBuilder();

            Serialize(sb, block.Lines, "", options);
            Serialize(sb, block.Rectangles, "", options);
            Serialize(sb, block.Ellipses, "", options);
            Serialize(sb, block.Texts, "", options);
            Serialize(sb, block.Images, "", options);
            Serialize(sb, block.Blocks, "", options);

            return sb.ToString();
        }

        public static string SerializeContents(BlockItem block)
        {
            return SerializeContents(block, ItemSerializeOptions.Default);
        }

        #endregion

        #region Deserialize

        private static LineItem DeserializeLine(string[] m)
        {
            var lineItem = new LineItem();
            lineItem.Id = int.Parse(m[1]);
            lineItem.X1 = double.Parse(m[2]);
            lineItem.Y1 = double.Parse(m[3]);
            lineItem.X2 = double.Parse(m[4]);
            lineItem.Y2 = double.Parse(m[5]);
            lineItem.Stroke = new ItemColor()
            {
                Alpha = int.Parse(m[6]),
                Red = int.Parse(m[7]),
                Green = int.Parse(m[8]),
                Blue = int.Parse(m[9])
            };
            return lineItem;
        }

        private static RectangleItem DeserializeRectangle(string[] m)
        {
            var rectangleItem = new RectangleItem();
            rectangleItem.Id = int.Parse(m[1]);
            rectangleItem.X = double.Parse(m[2]);
            rectangleItem.Y = double.Parse(m[3]);
            rectangleItem.Width = double.Parse(m[4]);
            rectangleItem.Height = double.Parse(m[5]);
            rectangleItem.IsFilled = bool.Parse(m[6]);
            rectangleItem.Stroke = new ItemColor()
            {
                Alpha = int.Parse(m[7]),
                Red = int.Parse(m[8]),
                Green = int.Parse(m[9]),
                Blue = int.Parse(m[10])
            };
            rectangleItem.Fill = new ItemColor()
            {
                Alpha = int.Parse(m[11]),
                Red = int.Parse(m[12]),
                Green = int.Parse(m[13]),
                Blue = int.Parse(m[14])
            };
            return rectangleItem;
        }

        private static EllipseItem DeserializeEllipse(string[] m)
        {
            var ellipseItem = new EllipseItem();
            ellipseItem.Id = int.Parse(m[1]);
            ellipseItem.X = double.Parse(m[2]);
            ellipseItem.Y = double.Parse(m[3]);
            ellipseItem.Width = double.Parse(m[4]);
            ellipseItem.Height = double.Parse(m[5]);
            ellipseItem.IsFilled = bool.Parse(m[6]);
            ellipseItem.Stroke = new ItemColor()
            {
                Alpha = int.Parse(m[7]),
                Red = int.Parse(m[8]),
                Green = int.Parse(m[9]),
                Blue = int.Parse(m[10])
            };
            ellipseItem.Fill = new ItemColor()
            {
                Alpha = int.Parse(m[11]),
                Red = int.Parse(m[12]),
                Green = int.Parse(m[13]),
                Blue = int.Parse(m[14])
            };
            return ellipseItem;
        }

        private static TextItem DeserializeText(string[] m)
        {
            var textItem = new TextItem();
            textItem.Id = int.Parse(m[1]);
            textItem.X = double.Parse(m[2]);
            textItem.Y = double.Parse(m[3]);
            textItem.Width = double.Parse(m[4]);
            textItem.Height = double.Parse(m[5]);
            textItem.HAlign = int.Parse(m[6]);
            textItem.VAlign = int.Parse(m[7]);
            textItem.Size = double.Parse(m[8]);
            textItem.Foreground = new ItemColor()
            {
                Alpha = int.Parse(m[9]),
                Red = int.Parse(m[10]),
                Green = int.Parse(m[11]),
                Blue = int.Parse(m[12])
            };
            textItem.Backgroud = new ItemColor()
            {
                Alpha = int.Parse(m[13]),
                Red = int.Parse(m[14]),
                Green = int.Parse(m[15]),
                Blue = int.Parse(m[16])
            };
            textItem.Text = m[17];
            return textItem;
        }

        private static ImageItem DeserializeImage(string[] m)
        {
            var imageItem = new ImageItem();
            imageItem.Id = int.Parse(m[1]);
            imageItem.X = double.Parse(m[2]);
            imageItem.Y = double.Parse(m[3]);
            imageItem.Width = double.Parse(m[4]);
            imageItem.Height = double.Parse(m[5]);
            imageItem.Data = Base64.ToBytes(m[6]);
            return imageItem;
        }

        private static BlockItem DeserializeBlockRecursive(string[] lines,
            int length,
            ref int end,
            string[] m,
            ItemSerializeOptions options)
        {
            var blockItem = DeserializeRootBlock(lines,
                length,
                ref end,
                m[4],
                int.Parse(m[1]),
                double.Parse(m[2]),
                double.Parse(m[3]),
                int.Parse(m[11]),
                options);

            blockItem.Width = double.Parse(m[5]);
            blockItem.Width = double.Parse(m[6]);
            blockItem.Backgroud = new ItemColor()
            {
                Alpha = int.Parse(m[7]),
                Red = int.Parse(m[8]),
                Green = int.Parse(m[9]),
                Blue = int.Parse(m[10])
            };
            blockItem.DataId = int.Parse(m[11]);
            return blockItem;
        }

        private static BlockItem DeserializeRootBlock(string[] lines,
            int length,
            ref int end,
            string name,
            int id,
            double x,
            double y,
            int dataId,
            ItemSerializeOptions options)
        {
            var root = new BlockItem();
            root.Init(id, x, y, dataId, name);

            for (; end < length; end++)
            {
                string line = lines[end].TrimStart(options.WhiteSpace);
                var m = line.Split(options.ModelSeparators);
                if (m.Length == 10 && string.Compare(m[0], "LINE", true) == 0)
                {
                    if (m.Length == 10)
                    {
                        var lineItem = DeserializeLine(m);
                        root.Lines.Add(lineItem);
                    }
                    else
                    {
                        throw new Exception(string.Format("Invalid LINE item at line {0}", end + 1));
                    }
                }
                else if (string.Compare(m[0], "RECTANGLE", true) == 0)
                {
                    if (m.Length == 15)
                    {
                        var rectangleItem = DeserializeRectangle(m);
                        root.Rectangles.Add(rectangleItem);
                    }
                    else
                    {
                        throw new Exception(string.Format("Invalid RECTANGLE item at line {0}", end + 1));
                    }
                }
                else if (string.Compare(m[0], "ELLIPSE", true) == 0)
                {
                    if (m.Length == 15)
                    {
                        var ellipseItem = DeserializeEllipse(m);
                        root.Ellipses.Add(ellipseItem);
                    }
                    else
                    {
                        throw new Exception(string.Format("Invalid ELLIPSE item at line {0}", end + 1));
                    }
                }
                else if (string.Compare(m[0], "TEXT", true) == 0)
                {
                    if (m.Length == 18)
                    {
                        var textItem = DeserializeText(m);
                        root.Texts.Add(textItem);
                    }
                    else
                    {
                        throw new Exception(string.Format("Invalid TEXT item at line {0}", end + 1));
                    }
                }
                else if (string.Compare(m[0], "IMAGE", true) == 0)
                {
                    if (m.Length == 7)
                    {
                        var imageItem = DeserializeImage(m);
                        root.Images.Add(imageItem);
                    }
                    else
                    {
                        throw new Exception(string.Format("Invalid IMAGE item at line {0}", end + 1));
                    }
                }
                else if (string.Compare(m[0], "BLOCK", true) == 0)
                {
                    if (m.Length == 12)
                    {
                        end++;
                        var blockItem = DeserializeBlockRecursive(lines, length, ref end, m, options);
                        root.Blocks.Add(blockItem);
                        continue;
                    }
                    else
                    {
                        throw new Exception(string.Format("Invalid BLOCK item at line {0}", end + 1));
                    }
                }
                else if (string.Compare(m[0], "END", true) == 0)
                {
                    if (m.Length == 1)
                    {
                        return root;
                    }
                    else
                    {
                        throw new Exception(string.Format("Invalid END item at line {0}", end + 1));
                    }
                }
                else if (m[0].StartsWith("//"))
                {
                    continue;
                }
                else
                {
                    throw new Exception(string.Format("Invalid item at line {0}", end + 1));
                }
            }
            return root;
        }

        public static BlockItem DeserializeContents(string model, ItemSerializeOptions options)
        {
            try
            {
                string[] lines = model.Split(options.LineSeparators, StringSplitOptions.RemoveEmptyEntries);
                int length = lines.Length;
                int end = 0;
                return DeserializeRootBlock(lines, length, ref end, "", 0, 0.0, 0.0, -1, options);
            }
            catch (Exception ex)
            {
                Debug.Print(ex.Message);
                Debug.Print(ex.StackTrace);
            }
            return null;
        }

        public static BlockItem DeserializeContents(string model)
        {
            return DeserializeContents(model, ItemSerializeOptions.Default);
        }

        #endregion
    }

    #endregion

    #region Item Controller

    public static class ItemController
    {
        #region Text

        public async static Task<string> OpenText(string fileName)
        {
            try
            {
                using (var stream = System.IO.File.OpenText(fileName))
                {
                    return await stream.ReadToEndAsync();
                }
            }
            catch (Exception ex)
            {
                Debug.Print(ex.Message);
                Debug.Print(ex.StackTrace);
            }
            return null;
        }

        public async static void SaveText(string fileName, string text)
        {
            try
            {
                if (text != null)
                {
                    using (var stream = System.IO.File.CreateText(fileName))
                    {
                        await stream.WriteAsync(text);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.Print(ex.Message);
                Debug.Print(ex.StackTrace);
            }
        }

        #endregion

        #region Position

        public static void ResetPosition(BlockItem block, double originX, double originY, double width, double height)
        {
            double minX = width;
            double minY = height;
            double maxX = originX;
            double maxY = originY;
            MinMax(block, ref minX, ref minY, ref maxX, ref maxY);
            double x = -(maxX - (maxX - minX));
            double y = -(maxY - (maxY - minY));
            Move(block, x, y);
        }

        #endregion

        #region MinMax

        public static void MinMax(BlockItem block, ref double minX, ref double minY, ref double maxX, ref double maxY)
        {
            MinMax(block.Lines, ref minX, ref minY, ref maxX, ref maxY);
            MinMax(block.Rectangles, ref minX, ref minY, ref maxX, ref maxY);
            MinMax(block.Ellipses, ref minX, ref minY, ref maxX, ref maxY);
            MinMax(block.Texts, ref minX, ref minY, ref maxX, ref maxY);
            MinMax(block.Images, ref minX, ref minY, ref maxX, ref maxY);
            MinMax(block.Blocks, ref minX, ref minY, ref maxX, ref maxY);
        }

        public static void MinMax(IEnumerable<BlockItem> blocks, ref double minX, ref double minY, ref double maxX, ref double maxY)
        {
            foreach (var block in blocks)
            {
                MinMax(block, ref minX, ref minY, ref maxX, ref maxY);
            }
        }

        public static void MinMax(IEnumerable<LineItem> lines, ref double minX, ref double minY, ref double maxX, ref double maxY)
        {
            foreach (var line in lines)
            {
                minX = Math.Min(minX, line.X1);
                minX = Math.Min(minX, line.X2);
                minY = Math.Min(minY, line.Y1);
                minY = Math.Min(minY, line.Y2);
                maxX = Math.Max(maxX, line.X1);
                maxX = Math.Max(maxX, line.X2);
                maxY = Math.Max(maxY, line.Y1);
                maxY = Math.Max(maxY, line.Y2);
            }
        }

        public static void MinMax(IEnumerable<RectangleItem> rectangles, ref double minX, ref double minY, ref double maxX, ref double maxY)
        {
            foreach (var rectangle in rectangles)
            {
                minX = Math.Min(minX, rectangle.X);
                minY = Math.Min(minY, rectangle.Y);
                maxX = Math.Max(maxX, rectangle.X);
                maxY = Math.Max(maxY, rectangle.Y);
            }
        }

        public static void MinMax(IEnumerable<EllipseItem> ellipses, ref double minX, ref double minY, ref double maxX, ref double maxY)
        {
            foreach (var ellipse in ellipses)
            {
                minX = Math.Min(minX, ellipse.X);
                minY = Math.Min(minY, ellipse.Y);
                maxX = Math.Max(maxX, ellipse.X);
                maxY = Math.Max(maxY, ellipse.Y);
            }
        }

        public static void MinMax(IEnumerable<TextItem> texts, ref double minX, ref double minY, ref double maxX, ref double maxY)
        {
            foreach (var text in texts)
            {
                minX = Math.Min(minX, text.X);
                minY = Math.Min(minY, text.Y);
                maxX = Math.Max(maxX, text.X);
                maxY = Math.Max(maxY, text.Y);
            }
        }

        public static void MinMax(IEnumerable<ImageItem> images, ref double minX, ref double minY, ref double maxX, ref double maxY)
        {
            foreach (var image in images)
            {
                minX = Math.Min(minX, image.X);
                minY = Math.Min(minY, image.Y);
                maxX = Math.Max(maxX, image.X);
                maxY = Math.Max(maxY, image.Y);
            }
        }

        #endregion

        #region Move

        public static void Move(IEnumerable<BlockItem> blocks, double x, double y)
        {
            foreach (var block in blocks)
            {
                Move(block, x, y);
            }
        }

        public static void Move(BlockItem block, double x, double y)
        {
            Move(block.Lines, x, y);
            Move(block.Rectangles, x, y);
            Move(block.Ellipses, x, y);
            Move(block.Texts, x, y);
            Move(block.Images, x, y);
            Move(block.Blocks, x, y);
        }

        public static void Move(IEnumerable<LineItem> lines, double x, double y)
        {
            foreach (var line in lines)
            {
                Move(line, x, y);
            }
        }

        public static void Move(IEnumerable<RectangleItem> rectangles, double x, double y)
        {
            foreach (var rectangle in rectangles)
            {
                Move(rectangle, x, y);
            }
        }

        public static void Move(IEnumerable<EllipseItem> ellipses, double x, double y)
        {
            foreach (var ellipse in ellipses)
            {
                Move(ellipse, x, y);
            }
        }

        public static void Move(IEnumerable<TextItem> texts, double x, double y)
        {
            foreach (var text in texts)
            {
                Move(text, x, y);
            }
        }

        public static void Move(IEnumerable<ImageItem> images, double x, double y)
        {
            foreach (var image in images)
            {
                Move(image, x, y);
            }
        }

        public static void Move(LineItem line, double x, double y)
        {
            line.X1 += x;
            line.Y1 += y;
            line.X2 += x;
            line.Y2 += y;
        }

        public static void Move(RectangleItem rectangle, double x, double y)
        {
            rectangle.X += x;
            rectangle.Y += y;
        }

        public static void Move(EllipseItem ellipse, double x, double y)
        {
            ellipse.X += x;
            ellipse.Y += y;
        }

        public static void Move(TextItem text, double x, double y)
        {
            text.X += x;
            text.Y += y;
        }

        public static void Move(ImageItem image, double x, double y)
        {
            image.X += x;
            image.Y += y;
        }

        #endregion

        #region Snap

        public static double Snap(double val, double snap)
        {
            double r = val % snap;
            return r >= snap / 2.0 ? val + snap - r : val - r;
        }

        #endregion
    }

    #endregion
}