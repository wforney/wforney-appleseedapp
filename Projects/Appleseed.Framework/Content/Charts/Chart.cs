using System.Drawing;

namespace Appleseed.Content.Charts
{
    //*********************************************************************
    //
    // Chart Class
    //
    // Base class implementation for BarChart and PieChart
    //
    //*********************************************************************

    /// <summary>
    /// One of the Graphing classes from the reporting asp.net starter kit
    /// on www.asp.net - http://www.asp.net/Default.aspx?tabindex=9&amp;tabID=47
    /// Made very minor changes to the code to use with monitoring module.
    /// Imported by Paul Yarrow, paul@paulyarrow.com
    /// </summary>
    public abstract class Chart
    {
        private const int _colorLimit = 25;

        private Color[] _color =
            {
                Color.ForestGreen,
                Color.Beige,
                Color.SlateBlue,
                Color.Brown,
                Color.Coral,
                Color.Crimson,
                Color.DarkCyan,
                Color.AliceBlue,
                Color.Gold,
                Color.Green,
                Color.BlueViolet,
                Color.HotPink,
                Color.Orange,
                Color.RoyalBlue,
                Color.Navy,
                Color.Olive,
                Color.Ivory,
                Color.Orchid,
                Color.PapayaWhip,
                Color.Pink,
                Color.Plum,
                Color.Red,
                Color.Goldenrod,
                Color.Salmon,
                Color.Blue
            };

        // Represent collection of all data points for the chart
        private ChartItemsCollection _dataPoints = new ChartItemsCollection();

        // The implementation of this method is provided by derived classes
        public abstract Bitmap Draw();

        /// <summary>
        /// Gets or sets the data points.
        /// </summary>
        /// <value>The data points.</value>
        public ChartItemsCollection DataPoints
        {
            get { return _dataPoints; }
            set { _dataPoints = value; }
        }

        /// <summary>
        /// Sets the color.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="NewColor">The new color.</param>
        public void SetColor(int index, Color NewColor)
        {
            // Changed for cycle. jviladiu@portalServices.net (05/07/2004)
            //if (index < _colorLimit) 
            //{
            //	_color[index] = NewColor;
            //}
            //else
            //{
            //	throw new Exception("Color Limit is " + _colorLimit);
            //}
            _color[index%_colorLimit] = NewColor;
        }

        /// <summary>
        /// Gets the color.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        public Color GetColor(int index)
        {
            // Changed for cycle. jviladiu@portalServices.net (05/07/2004)
            //if (index < _colorLimit) 
            //{
            //	return _color[index];
            //}
            //else
            //{
            //	throw new Exception("Color Limit is " + _colorLimit);
            //}
            return _color[index%_colorLimit];
        }
    }
}