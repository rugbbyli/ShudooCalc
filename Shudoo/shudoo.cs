using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Data;
using System.ComponentModel;

namespace Shudoo
{
    class Block
    {
        int[] possible;
        int index = 0;

        public Block() { }

        public void Clear()
        {
            index = 0;
            possible = null;
        }

        public int[] Possible { get { return possible; } set { possible = value; } }

        public int NextPossible()
        {
            int r = possible[index++];

            return r;

        }

        public bool HasPossible { get { return index < possible.Length; } }

    }

    public class MyConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            int v = (int)value;
            if (v == 0)
                return "";
            return v.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    class shudoo : INotifyPropertyChanged
    {
        int[,] data;
        Dictionary<int, Block> tmp;
        bool running;
      
        public event EventHandler Finish;
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }

        public int[,] Data { get { return data; } set { data = value; } }

        public bool Running { get { return running; } set { running = value; } }

        public bool NotSure(int location)
        {
            return tmp.ContainsKey(location);
        }

        public shudoo()
        {
            Clear();
            
        }

        public void Clear()
        {
            data = new int[9, 9];
            if (tmp == null)
                tmp = new Dictionary<int, Block>();
            else
            {
                tmp.Clear();
            }
        }

        public void Run()
        {
            if (running) return;
            ThreadPool.QueueUserWorkItem(delegate(object s)
            {
                if (!Init()) return;
                running = true;
                int index = 0;
                while (index >= 0 && index < 81 && running)
                {

                    if (tmp.ContainsKey(index))
                    {
                        tmp[index].Possible = AllPossible(index / 9, index % 9);

                        if (tmp[index].HasPossible)
                        {
                            data[index / 9, index % 9] = tmp[index].NextPossible();
                            index++;
                        }

                        else
                        {
                            tmp[index--].Clear();
                            while (!tmp.ContainsKey(index)) index--;
                            data[index / 9, index % 9] = 0;
                            //continue;
                        }
                    }
                    else index++;
                }
                if(Finish != null)
                    this.Finish(this, new EventArgs());
                running = false;
            });
        }

        public int[] AllPossible(int hang, int lie)
        {
            List<int> ans = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9 };

            for (int i = 0; i < 9; i++)
            {
                if (data[hang, i] != 0)
                    ans.Remove(data[hang, i]);
            }

            for (int i = 0; i < 9; i++)
            {
                if (data[i, lie] != 0)
                    ans.Remove(data[i, lie]);
            }

            for (int i = hang / 3 * 3; i < hang / 3 * 3 + 3; i++)
            {
                for (int j = lie / 3 * 3; j < lie / 3 * 3 + 3; j++)
                {
                    if (data[i, j] != 0)
                        ans.Remove(data[i, j]);
                }
            }

            return ans.ToArray();
        }

        bool Init()
        {
            tmp.Clear();
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    if (data[i, j] == 0)
                    {
                        int[] p = AllPossible(i, j);
                        if (p.Length == 0)
                        {
                            return false;
                        }
                        
                        else if (!tmp.ContainsKey(i * 9 + j))
                        {
                            Block b = new Block();
                            tmp.Add(i * 9 + j, b);
                        }
                    }
                }
            }
            return true;
        }

    }
}
