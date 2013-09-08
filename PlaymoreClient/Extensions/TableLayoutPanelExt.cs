using System;
using System.Runtime.CompilerServices;
using System.Windows.Forms;

namespace PlaymoreClient.Extensions
{
	public static class TableLayoutPanelExt
	{
		public static void AddControl(this TableLayoutPanel panel, Control control, int x, int y)
		{
			panel.EnsureSize(x + 1, y + 1);
			panel.Controls.Add(control, x, y);
		}

		public static void EnsureSize(this TableLayoutPanel panel, int columns, int rows)
		{
			while (panel.RowStyles.Count < rows)
			{
				panel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
			}
			while (panel.ColumnStyles.Count < columns)
			{
				panel.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
			}
			if (panel.RowCount < rows)
			{
				panel.RowCount = rows;
			}
			if (panel.ColumnCount < columns)
			{
				panel.ColumnCount = columns;
			}
		}
	}
}