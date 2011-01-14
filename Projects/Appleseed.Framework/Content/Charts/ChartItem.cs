using System.Collections;
using System.Drawing;

namespace Appleseed.Content.Charts
{
    //*********************************************************************
    //
    // ChartItem Class
    //
    // This class represents a data point in a chart
    //
    //*********************************************************************

    /// <summary>
    /// One of the Graphing classes from the reporting asp.net starter kit
    /// on www.asp.net - http://www.asp.net/Default.aspx?tabindex=9&amp;tabID=47
    /// Made very minor changes to the code to use with monitoring module.
    /// Imported by Paul Yarrow, paul@paulyarrow.com
    /// </summary>
    public class ChartItem
    {
        private string _label;
        private string _description;
        private float _value;
        private Color _color;
        private float _startPos;
        private float _sweepSize;

        private ChartItem()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:ChartItem"/> class.
        /// </summary>
        /// <param name="label">The label.</param>
        /// <param name="desc">The desc.</param>
        /// <param name="data">The data.</param>
        /// <param name="start">The start.</param>
        /// <param name="sweep">The sweep.</param>
        /// <param name="clr">The CLR.</param>
        public ChartItem(string label, string desc, float data, float start, float sweep, Color clr)
        {
            _label = label;
            _description = desc;
            _value = data;
            _startPos = start;
            _sweepSize = sweep;
            _color = clr;
        }

        /// <summary>
        /// Gets or sets the label.
        /// </summary>
        /// <value>The label.</value>
        public string Label
        {
            get { return _label; }
            set { _label = value; }
        }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        /// <value>The description.</value>
        public string Description
        {
            get { return _description; }
            set { _description = value; }
        }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>The value.</value>
        public float Value
        {
            get { return _value; }
            set { _value = value; }
        }

        /// <summary>
        /// Gets or sets the color of the item.
        /// </summary>
        /// <value>The color of the item.</value>
        public Color ItemColor
        {
            get { return _color; }
            set { _color = value; }
        }

        /// <summary>
        /// Gets or sets the start pos.
        /// </summary>
        /// <value>The start pos.</value>
        public float StartPos
        {
            get { return _startPos; }
            set { _startPos = value; }
        }

        /// <summary>
        /// Gets or sets the size of the sweep.
        /// </summary>
        /// <value>The size of the sweep.</value>
        public float SweepSize
        {
            get { return _sweepSize; }
            set { _sweepSize = value; }
        }
    }

    //*********************************************************************
    //
    // Custom Collection for ChartItems
    //
    //*********************************************************************

    public class ChartItemsCollection : CollectionBase
    {
        /// <summary>
        /// Gets or sets the <see cref="T:ChartItem"/> at the specified index.
        /// </summary>
        /// <value></value>
        public ChartItem this[int index]
        {
            get { return (ChartItem) (List[index]); }
            set { List[index] = value; }
        }

        /// <summary>
        /// Adds the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public int Add(ChartItem value)
        {
            return List.Add(value);
        }

        /// <summary>
        /// Indexes the of.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public int IndexOf(ChartItem value)
        {
            return List.IndexOf(value);
        }

        /// <summary>
        /// Determines whether [contains] [the specified value].
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>
        /// 	<c>true</c> if [contains] [the specified value]; otherwise, <c>false</c>.
        /// </returns>
        public bool Contains(ChartItem value)
        {
            return List.Contains(value);
        }

        /// <summary>
        /// Removes the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        public void Remove(ChartItem value)
        {
            List.Remove(value);
        }
    }
}