using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Shudoo
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        shudoo sd = new shudoo();
        int curRow = 0, curCol = 0;
        MyVisualHost mvh = new MyVisualHost();
        Typeface tf;
        double fontSize = 28f;

        public MainWindow()
        {
            InitializeComponent();
            sd.Finish += new EventHandler(delegate(object s, EventArgs ea)
            {
                this.Dispatcher.Invoke(new FinishDelegate(Finish), null);
            });
            canvas.Children.Add(mvh);
            Canvas.SetLeft(mvh, 16);
            Canvas.SetTop(mvh, 7);
            tf = new Typeface(txt.FontFamily, txt.FontStyle, txt.FontWeight, txt.FontStretch);
        }

        private void Canvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (txt.Visibility == System.Windows.Visibility.Visible)
            {
                int num;
                if (int.TryParse(txt.Text, out num))
                {
                    if (num !=0 && !sd.AllPossible(curRow, curCol).Contains(num))
                    {
                        MessageBox.Show(num + "在（" + (curRow + 1) + "，" + (curCol + 1) + "）不是一个合法的输入值！");
                        return;
                    }
                    sd.Data[curRow, curCol] = num;
                }               
            }
            curRow = (int)e.GetPosition(canvas).X / 50;
            curCol = (int)e.GetPosition(canvas).Y / 50;
            //else
            {
                
                txt.Text = sd.Data[curRow, curCol].ToString();

                sd.Data[curRow, curCol] = 0;

                Refresh();

                Canvas.SetLeft(txt, curRow * 50);
                Canvas.SetTop(txt, curCol * 50);

                txt.Visibility = System.Windows.Visibility.Visible;
            }
        }

        private void Window_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (txt.Visibility == System.Windows.Visibility.Visible)
            {
                int n = e.Delta > 0 ? 1 : -1;
                int num = Convert.ToInt32(txt.Text) + n;
                if(num < 0) num = 0;
                else if( num > 9)    num = 9;
                txt.Text = (num).ToString();
            }
        }

        void Refresh()
        {
            DrawingVisual dv = new DrawingVisual();
            DrawingContext dc = dv.RenderOpen();
            
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    if (sd.Data[i, j] != 0)
                    {
                        Brush b = Brushes.Red;
                        if (sd.NotSure(i * 9 + j)) b = Brushes.Green;
                        dc.DrawText(
                            new FormattedText(sd.Data[i, j].ToString(), 
                                            System.Globalization.CultureInfo.CurrentCulture, 
                                            FlowDirection.LeftToRight, 
                                            tf, 
                                            fontSize, 
                                            b),
                            new Point(i * 50, j * 50));
                    }
                }
            }
            dc.Close();
            mvh.Clear(dv);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (sender == buttonGo)
            {
                expander.IsEnabled = false;
                expander.IsExpanded = false;
                sd.Run();
            }
            else if(sender == buttonClear)
            {
                sd.Clear();
                Refresh();
            }
            else if (sender == buttonSave)
            {
                #region Draw Image
                DrawingVisual dv = new DrawingVisual();
                DrawingContext dc = dv.RenderOpen();
                dc.DrawImage(((ImageBrush)canvas.Background).ImageSource, new Rect(0, 0, 460, 460));
                for (int i = 0; i < 9; i++)
                {
                    for (int j = 0; j < 9; j++)
                    {
                        if (sd.Data[i, j] != 0)
                        {
                            Brush b = Brushes.Red;
                            if (sd.NotSure(i * 9 + j)) b = Brushes.Green;
                            dc.DrawText(
                                new FormattedText(sd.Data[i, j].ToString(),
                                                System.Globalization.CultureInfo.CurrentCulture,
                                                FlowDirection.LeftToRight,
                                                tf,
                                                fontSize,
                                                b),
                                new Point(i * 50 + 20, j * 50 + 11));
                        }
                    }
                }
                dc.Close();
                #endregion
                RenderTargetBitmap rtb = new RenderTargetBitmap(460,460,96,96,PixelFormats.Pbgra32);
                rtb.Render(dv);
                Clipboard.SetImage(rtb);
                MessageBox.Show("图像已复制到剪切板");
            }
        }

        delegate void FinishDelegate();
        void Finish()
        {
            Refresh();
            expander.IsEnabled = true;
            expander.IsExpanded = true;
        }

        private void Window_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

            if (e.OriginalSource == canvas) return;
            if (txt.Visibility == System.Windows.Visibility.Hidden) return;

            {
                
                int num;
                if (int.TryParse(txt.Text, out num))
                {
                    if (num != 0 && !sd.AllPossible(curRow, curCol).Contains(num))
                    {
                        MessageBox.Show(num + "在（" + (curRow + 1) + "，" + (curCol + 1) + "）不是一个合法的输入值，已舍弃输入！");
                    }
                    else sd.Data[curRow, curCol] = num;
                }
                Refresh();
                txt.Visibility = System.Windows.Visibility.Hidden;
                
            }
        }
    }
}
