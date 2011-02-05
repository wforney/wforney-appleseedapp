using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace Appleseed.Content.Charts
{
    //*********************************************************************
    //
    // BarGraph Class
    //
    // This class uses GDI+ to render Bar Chart.
    //
    //*********************************************************************

    /// <summary>
    /// One of the Graphing classes from the reporting asp.net starter kit
    /// on www.asp.net - http://www.asp.net/Default.aspx?tabindex=9&amp;tabID=47
    /// Made very minor changes to the code to use with monitoring module.
    /// Imported by Paul Yarrow, paul@paulyarrow.com
    /// </summary>
    public class BarGraph : Chart
    {
        private const float _graphLegendSpacer = 15F;
        private const int _labelFontSize = 7;
        private const int _legendFontSize = 9;
        private const float _legendRectangleSize = 10F;
        private const float _spacer = 5F;

        // Overall related members
        private Color _backColor;
        private string _fontFamily;
        private string _longestTickValue = string.Empty; // Used to calculate max value width
        private float _maxTickValueWidth; // Used to calculate left offset of bar graph
        private float _totalHeight;
        private float _totalWidth;

        // Graph related members
        private float _barWidth;
        private float _bottomBuffer; // Space from bottom to x axis
        private bool _displayBarData;
        private Color _fontColor;
        private float _graphHeight;
        private float _graphWidth;
        private float _maxValue = 0.0f; // = final tick value * tick count
        private float _scaleFactor; // = _maxValue / _graphHeight
        private float _spaceBtwBars; // For now same as _barWidth
        private float _topBuffer; // Space from top to the top of y axis
        private float _xOrigin; // x position where graph starts drawing
        private float _yOrigin; // y position where graph starts drawing
        private string _yLabel;
        private int _yTickCount;
        private float _yTickValue; // Value for each tick = _maxValue/_yTickCount

        // Legend related members
        private bool _displayLegend;
        private float _legendWidth;
        private string _longestLabel = string.Empty; // Used to calculate legend width
        private float _maxLabelWidth = 0.0f;

        /// <summary>
        /// Gets or sets the font family.
        /// </summary>
        /// <value>The font family.</value>
        public string FontFamily
        {
            get { return _fontFamily; }
            set { _fontFamily = value; }
        }

        /// <summary>
        /// Sets the color of the background.
        /// </summary>
        /// <value>The color of the background.</value>
        public Color BackgroundColor
        {
            set { _backColor = value; }
        }

        /// <summary>
        /// Sets the bottom buffer.
        /// </summary>
        /// <value>The bottom buffer.</value>
        public int BottomBuffer
        {
            set { _bottomBuffer = Convert.ToSingle(value); }
        }

        /// <summary>
        /// Sets the color of the font.
        /// </summary>
        /// <value>The color of the font.</value>
        public Color FontColor
        {
            set { _fontColor = value; }
        }

        /// <summary>
        /// Gets or sets the height.
        /// </summary>
        /// <value>The height.</value>
        public int Height
        {
            get { return Convert.ToInt32(_totalHeight); }
            set { _totalHeight = Convert.ToSingle(value); }
        }

        /// <summary>
        /// Gets or sets the width.
        /// </summary>
        /// <value>The width.</value>
        public int Width
        {
            get { return Convert.ToInt32(_totalWidth); }
            set { _totalWidth = Convert.ToSingle(value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [show legend].
        /// </summary>
        /// <value><c>true</c> if [show legend]; otherwise, <c>false</c>.</value>
        public bool ShowLegend
        {
            get { return _displayLegend; }
            set { _displayLegend = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [show data].
        /// </summary>
        /// <value><c>true</c> if [show data]; otherwise, <c>false</c>.</value>
        public bool ShowData
        {
            get { return _displayBarData; }
            set { _displayBarData = value; }
        }

        /// <summary>
        /// Sets the top buffer.
        /// </summary>
        /// <value>The top buffer.</value>
        public int TopBuffer
        {
            set { _topBuffer = Convert.ToSingle(value); }
        }

        /// <summary>
        /// Gets or sets the vertical label.
        /// </summary>
        /// <value>The vertical label.</value>
        public string VerticalLabel
        {
            get { return _yLabel; }
            set { _yLabel = value; }
        }

        /// <summary>
        /// Gets or sets the vertical tick count.
        /// </summary>
        /// <value>The vertical tick count.</value>
        public int VerticalTickCount
        {
            get { return _yTickCount; }
            set { _yTickCount = value; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:BarGraph"/> class.
        /// </summary>
        public BarGraph()
        {
            AssignDefaultSettings();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:BarGraph"/> class.
        /// </summary>
        /// <param name="bgColor">Color of the bg.</param>
        public BarGraph(Color bgColor)
        {
            AssignDefaultSettings();
            BackgroundColor = bgColor;
        }

        //*********************************************************************
        //
        // This method collects all data points and calculate all the necessary dimensions 
        // to draw the bar graph.  It is the method called before invoking the Draw() method.
        // labels is the x values.
        // values is the y values.
        //
        //*********************************************************************

        /// <summary>
        /// Collects the data points.
        /// </summary>
        /// <param name="labels">The labels.</param>
        /// <param name="values">The values.</param>
        public void CollectDataPoints(string[] labels, string[] values)
        {
            if (labels.Length == values.Length)
            {
                for (int i = 0; i < labels.Length; i++)
                {
                    float temp = Convert.ToSingle(values[i]);
                    string shortLbl = MakeShortLabel(labels[i]);

                    // For now put 0.0 for start position and sweep size
                    DataPoints.Add(new ChartItem(shortLbl, labels[i], temp, 0.0f, 0.0f, GetColor(i)));

                    // Find max value from data; this is only temporary _maxValue
                    if (_maxValue < temp) _maxValue = temp;

                    // Find the longest description
                    if (_displayLegend)
                    {
                        string currentLbl = labels[i] + " (" + shortLbl + ")";
                        float currentWidth = CalculateImgFontWidth(currentLbl, _legendFontSize, FontFamily);
                        if (_maxLabelWidth < currentWidth)
                        {
                            _longestLabel = currentLbl;
                            _maxLabelWidth = currentWidth;
                        }
                    }
                }

                CalculateTickAndMax();
                CalculateGraphDimension();
                CalculateBarWidth(DataPoints.Count, _graphWidth);
                CalculateSweepValues();
            }
            else
                throw new Exception("X data count is different from Y data count");
        }

        //*********************************************************************
        //
        // Same as above; called when user doesn't care about the x values
        //
        //*********************************************************************

        /// <summary>
        /// Collects the data points.
        /// </summary>
        /// <param name="values">The values.</param>
        public void CollectDataPoints(string[] values)
        {
            string[] labels = values;
            CollectDataPoints(labels, values);
        }

        //*********************************************************************
        //
        // This method returns a bar graph bitmap to the calling function.  It is called after 
        // all dimensions and data points are calculated.
        //
        //*********************************************************************

        /// <summary>
        /// Draws this instance.
        /// </summary>
        /// <returns></returns>
        public override Bitmap Draw()
        {
            int height = Convert.ToInt32(_totalHeight);
            int width = Convert.ToInt32(_totalWidth);

            Bitmap bmp = new Bitmap(width, height);

            using (Graphics graph = Graphics.FromImage(bmp))
            {
                graph.CompositingQuality = CompositingQuality.HighQuality;
                graph.SmoothingMode = SmoothingMode.AntiAlias;

                // Set the background: need to draw one pixel larger than the bitmap to cover all area
                graph.FillRectangle(new SolidBrush(_backColor), -1, -1, bmp.Width + 1, bmp.Height + 1);

                DrawVerticalLabelArea(graph);
                DrawBars(graph);
                DrawXLabelArea(graph);
                if (_displayLegend) DrawLegend(graph);
            }

            return bmp;
        }

        //*********************************************************************
        //
        // This method draws all the bars for the graph.
        //
        //*********************************************************************

        /// <summary>
        /// Draws the bars.
        /// </summary>
        /// <param name="graph">The graph.</param>
        private void DrawBars(Graphics graph)
        {
            SolidBrush brsFont = null;
            Font valFont = null;
            StringFormat sfFormat = null;

            try
            {
                brsFont = new SolidBrush(_fontColor);
                valFont = new Font(_fontFamily, _labelFontSize);
                sfFormat = new StringFormat();
                sfFormat.Alignment = StringAlignment.Center;
                int i = 0;

                // Draw bars and the value above each bar
                foreach (ChartItem item in DataPoints)
                {
                    using (SolidBrush barBrush = new SolidBrush(item.ItemColor))
                    {
                        float itemY = _yOrigin + _graphHeight - item.SweepSize;

                        // When drawing, all position is relative to (_xOrigin, _yOrigin)
                        graph.FillRectangle(barBrush, _xOrigin + item.StartPos, itemY, _barWidth, item.SweepSize);

                        // Draw data value
                        if (_displayBarData)
                        {
                            float startX = _xOrigin + (i*(_barWidth + _spaceBtwBars));
                                // This draws the value on center of the bar
                            float startY = itemY - 2f - valFont.Height; // Positioned on top of each bar by 2 pixels
                            RectangleF recVal =
                                new RectangleF(startX, startY, _barWidth + _spaceBtwBars, valFont.Height);
                            graph.DrawString(item.Value.ToString("#,###.##"), valFont, brsFont, recVal, sfFormat);
                        }
                        i++;
                    }
                }
            }
            finally
            {
                if (brsFont != null) brsFont.Dispose();
                if (valFont != null) valFont.Dispose();
                if (sfFormat != null) sfFormat.Dispose();
            }
        }

        //*********************************************************************
        //
        // This method draws the y label, tick marks, tick values, and the y axis.
        //
        //*********************************************************************

        /// <summary>
        /// Draws the vertical label area.
        /// </summary>
        /// <param name="graph">The graph.</param>
        private void DrawVerticalLabelArea(Graphics graph)
        {
            Font lblFont = null;
            SolidBrush brs = null;
            StringFormat lblFormat = null;
            Pen pen = null;
            StringFormat sfVLabel = null;

            try
            {
                lblFont = new Font(_fontFamily, _labelFontSize);
                brs = new SolidBrush(_fontColor);
                lblFormat = new StringFormat();
                pen = new Pen(_fontColor);
                sfVLabel = new StringFormat();

                lblFormat.Alignment = StringAlignment.Near;

                // Draw vertical label at the top of y-axis and place it in the middle top of y-axis
                RectangleF recVLabel =
                    new RectangleF(0f, _yOrigin - 2*_spacer - lblFont.Height, _xOrigin*2, lblFont.Height);
                sfVLabel.Alignment = StringAlignment.Center;
                graph.DrawString(_yLabel, lblFont, brs, recVLabel, sfVLabel);

                // Draw all tick values and tick marks
                for (int i = 0; i < _yTickCount; i++)
                {
                    float currentY = _topBuffer + (i*_yTickValue/_scaleFactor); // Position for tick mark
                    float labelY = currentY - lblFont.Height/2; // Place label in the middle of tick
                    RectangleF lblRec = new RectangleF(_spacer, labelY, _maxTickValueWidth, lblFont.Height);

                    float currentTick = _maxValue - i*_yTickValue; // Calculate tick value from top to bottom
                    graph.DrawString(currentTick.ToString("#,###.##"), lblFont, brs, lblRec, lblFormat);
                    // Draw tick value  
                    graph.DrawLine(pen, _xOrigin, currentY, _xOrigin - 4.0f, currentY); // Draw tick mark
                }

                // Draw y axis
                graph.DrawLine(pen, _xOrigin, _yOrigin, _xOrigin, _yOrigin + _graphHeight);
            }
            finally
            {
                if (lblFont != null) lblFont.Dispose();
                if (brs != null) brs.Dispose();
                if (lblFormat != null) lblFormat.Dispose();
                if (pen != null) pen.Dispose();
                if (sfVLabel != null) sfVLabel.Dispose();
            }
        }

        //*********************************************************************
        //
        // This method draws x axis and all x labels
        //
        //*********************************************************************

        /// <summary>
        /// Draws the X label area.
        /// </summary>
        /// <param name="graph">The graph.</param>
        private void DrawXLabelArea(Graphics graph)
        {
            Font lblFont = null;
            SolidBrush brs = null;
            StringFormat lblFormat = null;
            Pen pen = null;

            try
            {
                lblFont = new Font(_fontFamily, _labelFontSize);
                brs = new SolidBrush(_fontColor);
                lblFormat = new StringFormat();
                pen = new Pen(_fontColor);

                lblFormat.Alignment = StringAlignment.Center;

                // Draw x axis
                graph.DrawLine(pen, _xOrigin, _yOrigin + _graphHeight, _xOrigin + _graphWidth, _yOrigin + _graphHeight);

                float currentX;
                float currentY = _yOrigin + _graphHeight + 2.0f; // All x labels are drawn 2 pixels below x-axis
                float labelWidth = _barWidth + _spaceBtwBars; // Fits exactly below the bar
                int i = 0;

                // Draw x labels
                foreach (ChartItem item in DataPoints)
                {
                    currentX = _xOrigin + (i*labelWidth);
                    RectangleF recLbl = new RectangleF(currentX, currentY, labelWidth, lblFont.Height);
                    string lblString = _displayLegend ? item.Label : item.Description;
                        // Decide what to show: short or long

                    graph.DrawString(lblString, lblFont, brs, recLbl, lblFormat);
                    i++;
                }
            }
            finally
            {
                if (lblFont != null) lblFont.Dispose();
                if (brs != null) brs.Dispose();
                if (lblFormat != null) lblFormat.Dispose();
                if (pen != null) pen.Dispose();
            }
        }

        //*********************************************************************
        //
        // This method determines where to place the legend box.
        // It draws the legend border, legend description, and legend color code.
        //
        //*********************************************************************

        /// <summary>
        /// Draws the legend.
        /// </summary>
        /// <param name="graph">The graph.</param>
        private void DrawLegend(Graphics graph)
        {
            Font lblFont = null;
            SolidBrush brs = null;
            StringFormat lblFormat = null;
            Pen pen = null;

            try
            {
                lblFont = new Font(_fontFamily, _legendFontSize);
                brs = new SolidBrush(_fontColor);
                lblFormat = new StringFormat();
                pen = new Pen(_fontColor);
                lblFormat.Alignment = StringAlignment.Near;

                // Calculate Legend drawing start point
                float startX = _xOrigin + _graphWidth + _graphLegendSpacer;
                float startY = _yOrigin;

                float xColorCode = startX + _spacer;
                float xLegendText = xColorCode + _legendRectangleSize + _spacer;
                float legendHeight = 0.0f;
                for (int i = 0; i < DataPoints.Count; i++)
                {
                    ChartItem point = DataPoints[i];
                    string text = point.Description + " (" + point.Label + ")";
                    float currentY = startY + _spacer + (i*(lblFont.Height + _spacer));
                    legendHeight += lblFont.Height + _spacer;

                    // Draw legend description
                    graph.DrawString(text, lblFont, brs, xLegendText, currentY, lblFormat);

                    // Draw color code
                    graph.FillRectangle(new SolidBrush(DataPoints[i].ItemColor), xColorCode, currentY + 3f,
                                        _legendRectangleSize, _legendRectangleSize);
                }

                // Draw legend border
                graph.DrawRectangle(pen, startX, startY, _legendWidth, legendHeight + _spacer);
            }
            finally
            {
                if (lblFont != null) lblFont.Dispose();
                if (brs != null) brs.Dispose();
                if (lblFormat != null) lblFormat.Dispose();
                if (pen != null) pen.Dispose();
            }
        }

        //*********************************************************************
        //
        // This method calculates all measurement aspects of the bar graph from the given data points
        //
        //*********************************************************************

        /// <summary>
        /// Calculates the graph dimension.
        /// </summary>
        private void CalculateGraphDimension()
        {
            FindLongestTickValue();

            // Need to add another character for spacing; this is not used for drawing, just for calculation
            _longestTickValue += "0";
            _maxTickValueWidth = CalculateImgFontWidth(_longestTickValue, _labelFontSize, FontFamily);
            float leftOffset = _spacer + _maxTickValueWidth;
            float rtOffset = 0.0f;

            if (_displayLegend)
            {
                _legendWidth = _spacer + _legendRectangleSize + _spacer + _maxLabelWidth + _spacer;
                rtOffset = _graphLegendSpacer + _legendWidth + _spacer;
            }
            else
                rtOffset = _spacer; // Make graph in the middle

            _graphHeight = _totalHeight - _topBuffer - _bottomBuffer; // Buffer spaces are used to print labels
            _graphWidth = _totalWidth - leftOffset - rtOffset;
            _xOrigin = leftOffset;
            _yOrigin = _topBuffer;

            // Once the correct _maxValue is determined, then calculate _scaleFactor
            _scaleFactor = _maxValue/_graphHeight;
        }

        //*********************************************************************
        //
        // This method determines the longest tick value from the given data points.
        // The result is needed to calculate the correct graph dimension.
        //
        //*********************************************************************

        /// <summary>
        /// Finds the longest tick value.
        /// </summary>
        private void FindLongestTickValue()
        {
            float currentTick;
            string tickString;
            for (int i = 0; i < _yTickCount; i++)
            {
                currentTick = _maxValue - i*_yTickValue;
                tickString = currentTick.ToString("#,###.##");
                if (_longestTickValue.Length < tickString.Length)
                    _longestTickValue = tickString;
            }
        }

        //*********************************************************************
        //
        // This method calculates the image width in pixel for a given text
        //
        //*********************************************************************

        /// <summary>
        /// Calculates the width of the img font.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="size">The size.</param>
        /// <param name="family">The family.</param>
        /// <returns></returns>
        private float CalculateImgFontWidth(string text, int size, string family)
        {
            Bitmap bmp = null;
            Graphics graph = null;
            Font font = null;

            try
            {
                font = new Font(family, size);

                // Calculate the size of the string.
                bmp = new Bitmap(1, 1, PixelFormat.Format32bppArgb);
                graph = Graphics.FromImage(bmp);
                SizeF oSize = graph.MeasureString(text, font);

                return oSize.Width;
            }
            finally
            {
                if (graph != null) graph.Dispose();
                if (bmp != null) bmp.Dispose();
                if (font != null) font.Dispose();
            }
        }

        //*********************************************************************
        //
        // This method creates abbreviation from long description; used for making legend
        //
        //*********************************************************************

        /// <summary>
        /// Makes the short label.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns></returns>
        private string MakeShortLabel(string text)
        {
            string label = text;
            if (text.Length > 2)
            {
                int midPostition = Convert.ToInt32(Math.Floor((double) text.Length/2));
                label = text.Substring(0, 1) + text.Substring(midPostition, 1) + text.Substring(text.Length - 1, 1);
            }
            return label;
        }

        //*********************************************************************
        //
        // This method calculates the max value and each tick mark value for the bar graph.
        //
        //*********************************************************************

        /// <summary>
        /// Calculates the tick and max.
        /// </summary>
        private void CalculateTickAndMax()
        {
            float tempMax = 0.0f;

            // Give graph some head room first about 10% of current max
            _maxValue *= 1.1f;

            if (_maxValue != 0.0f)
            {
                // Find a rounded value nearest to the current max value
                // Calculate this max first to give enough space to draw value on each bar
                double exp = Convert.ToDouble(Math.Floor(Math.Log10(_maxValue)));
                tempMax = Convert.ToSingle(Math.Ceiling(_maxValue/Math.Pow(10, exp))*Math.Pow(10, exp));
            }
            else
                tempMax = 1.0f;

            // Once max value is calculated, tick value can be determined; tick value should be a whole number
            _yTickValue = tempMax/_yTickCount;
            double expTick = Convert.ToDouble(Math.Floor(Math.Log10(_yTickValue)));
            _yTickValue = Convert.ToSingle(Math.Ceiling(_yTickValue/Math.Pow(10, expTick))*Math.Pow(10, expTick));

            // Re-calculate the max value with the new tick value
            _maxValue = _yTickValue*_yTickCount;
        }

        //*********************************************************************
        //
        // This method calculates the height for each bar in the graph
        //
        //*********************************************************************

        private void CalculateSweepValues()
        {
            // Called when all values and scale factor are known
            // All values calculated here are relative from (_xOrigin, _yOrigin)
            int i = 0;
            foreach (ChartItem item in DataPoints)
            {
                // This implementation does not support negative value
                if (item.Value >= 0) item.SweepSize = item.Value/_scaleFactor;

                // (_spaceBtwBars/2) makes half white space for the first bar
                item.StartPos = (_spaceBtwBars/2) + i*(_barWidth + _spaceBtwBars);
                i++;
            }
        }

        //*********************************************************************
        //
        // This method calculates the width for each bar in the graph
        //
        //*********************************************************************

        /// <summary>
        /// Calculates the width of the bar.
        /// </summary>
        /// <param name="dataCount">The data count.</param>
        /// <param name="barGraphWidth">Width of the bar graph.</param>
        private void CalculateBarWidth(int dataCount, float barGraphWidth)
        {
            // White space between each bar is the same as bar width itself
            _barWidth = barGraphWidth/(dataCount*2); // Each bar has 1 white space 
            _spaceBtwBars = _barWidth;
        }

        //*********************************************************************
        //
        // This method assigns default value to the bar graph properties and is only 
        // called from BarGraph constructors
        //
        //*********************************************************************

        /// <summary>
        /// Assigns the default settings.
        /// </summary>
        private void AssignDefaultSettings()
        {
            // default values
            _totalWidth = 700f;
            _totalHeight = 450f;
            _fontFamily = "Verdana";
            _backColor = Color.White;
            _fontColor = Color.Black;
            _topBuffer = 30f;
            _bottomBuffer = 30f;
            _yTickCount = 2;
            _displayLegend = false;
            _displayBarData = false;
        }
    }
}