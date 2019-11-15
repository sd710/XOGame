using System.Drawing;
using System.Windows.Forms;

/*
 * Выводить, какой игрок победил
 * Начинать проверку на конец игры с 5 шага
 * */

namespace XOGame
{
    public partial class XOGame : Form
    {
        enum XO {Clear, X, O};
        XO nextFieldVal = XO.X; // первый ход за 'Х'
        XO[,] playingField;
        Graphics g;


        public XOGame()
        {
            InitializeComponent();

            //0 - пусто
            //1 - X
            //2 - O
            playingField = new XO[3, 3];
            g = Graphics.FromHwnd(this.Handle);
        }

        //Отрисовка поля
        private void XOGame_Paint(object sender, PaintEventArgs e)
        {
            DrawGameCoord();
        }

        //обработка кликов лкм/пкм
        private void XOGame_MouseClick(object sender, MouseEventArgs e)
        {
            NextStep(e);
        }

        //отрисовка х
        private void DrawX(Point mouseCoord)
        {
            Pen pen = new Pen(Color.Red, 3);
            //поле для игры делится на равные квадраты 3x3
            int x = ClientSize.Width / 3;
            int y = ClientSize.Height / 3;

            //MessageBox.Show(string.Format("{0}\n X: {1}, Y: {2}", p.ToString(), (int)(p.X / x), (int)(p.Y / y)));
            // (int)(p.X / x), (int)(p.Y / y)

            //определяем в каком квадрате кликнул игрок
            int fieldX = mouseCoord.X / x;
            int fieldY = mouseCoord.Y / y;

            /*
             * 00   10  20 
             * 10   11  21
             * 20   12  22
             */

            //x * (fieldX + 0), y * (fieldY + 0) - координаты левого верхнего угла, квадрата, в котором кликнул игрок
            //Рисуем X в соответствующем поле, если оно пустое
            if (playingField[fieldX, fieldY] == XO.Clear)
            {
                g.DrawLine(pen, x * (fieldX + 0), y * (fieldY + 0), x * (fieldX + 1), y * (fieldY + 1));
                g.DrawLine(pen, x * (fieldX + 1), y * (fieldY + 0), x * (fieldX + 0), y * (fieldY + 1));
                playingField[fieldX, fieldY] = XO.X;
                
            }
            else
                MessageBox.Show("Эта клетка не пустая");
        }

        //отрисовка о
        private void DrawO(Point mouseCoord)
        {
            
            Pen pen = new Pen(Color.Blue, 3);
            //поле для игры делится на равные квадраты 3x3
            int x = ClientSize.Width / 3;
            int y = ClientSize.Height / 3;

            //MessageBox.Show(string.Format("{0}\n X: {1}, Y: {2}", p.ToString(), (int)(p.X / x), (int)(p.Y / y)));
            // (int)(p.X / x), (int)(p.Y / y)

            //определяем в каком квадрате кликнул игрок
            int fieldX = mouseCoord.X / x;
            int fieldY = mouseCoord.Y / y;

            /*
             * 00   10  20 
             * 10   11  21
             * 20   12  22
             */

            //x * fieldX,  y * fieldY - координаты левого верхнего угла квадрата в котором кликнул игрок
            if (playingField[fieldX, fieldY] == XO.Clear)
            {
                g.DrawEllipse(pen, new Rectangle(x * fieldX, y * fieldY, x, y));
                playingField[fieldX, fieldY] = XO.O;
            }
            else
                MessageBox.Show("Эта клетка не пустая");
        }

        //проверка на заполненность всех полей
        private bool GameOver()
        {
            for (int i = 0; i < 3; i++)
                for (int j = 0; j < 3; j++)
                    if (playingField[i, j] == 0) return false;
            return true;
        }

        //отрисовка чистого поля
        private void DrawGameCoord()
        {
            Pen pen = new Pen(Color.FromArgb(255, 0, 0, 0));
            g.Clear(DefaultBackColor);
            for (int i = 0; i < 2; i++)
            {
                //vertical line
                g.DrawLine(pen, ClientSize.Width / 3 * (i + 1), 0, ClientSize.Width / 3 * (i + 1), ClientSize.Height);
                //horizontal line
                g.DrawLine(pen, 0, ClientSize.Height / 3 * (i + 1), ClientSize.Width, ClientSize.Height / 3 * (i + 1));
            }
        }

        //очистка массива заполненных полей и отрисовка игрового поля
        private void ClearPlayingField()
        {
            for (int i = 0; i < 3; i++)
                for (int j = 0; j < 3; j++)
                    playingField[i, j] = XO.Clear;
            g.Clear(DefaultBackColor);
            nextFieldVal = XO.X;
            DrawGameCoord(); //рисуем заново
        }

        //шаг игрока
        private void NextStep(MouseEventArgs e)
        {
            Point mouseCoord = new Point(e.X, e.Y);//координаты клика относительно пользовательской части формы
            //тут nextFieldVal - последний шаг, т.е. текущая фигура на поле
            switch (nextFieldVal)
            {
                case XO.X:
                    DrawX(mouseCoord);
                    //после отрисовки меняем на след.
                    nextFieldVal = XO.O;
                    break;
                case XO.O:
                    DrawO(mouseCoord);
                    nextFieldVal = XO.X;
                    break;
            }

            //проверка на окончание игры, после каждого шага
            if (GameOver())
            {
                MessageBox.Show("Конец игры");
                //playingField[0, 0] = XO.Clear;
                ClearPlayingField();//очищаем и рисуем заново внутри этой функ
            }
            else if (Winner(nextFieldVal == XO.X ? XO.O : XO.X))
            {
                MessageBox.Show(string.Format("Конец игры, победил {0}", nextFieldVal == XO.X ? XO.O.ToString() : XO.X.ToString()));
                ClearPlayingField();//очищаем и рисуем заново внутри этой функ
            }
        }

        //определяем победителя, нужно после 5-ого шага
        private bool Winner(XO figure)
        {
            //diagonal
            if (playingField[0, 0] == figure && playingField[1, 1] == figure && playingField[2, 2] == figure ||
                playingField[0, 2] == figure && playingField[1, 1] == figure && playingField[2, 0] == figure)
                return true;
            //Vertical, horizontal
            for (int i = 0; i < 3; i++)
            {
                if (playingField[i, 0] == figure && playingField[i, 1] == figure && playingField[i, 2] == figure ||
                playingField[0, i] == figure && playingField[1, i] == figure && playingField[2, i] == figure)
                    return true;
            }
            return false;
        }
    }
}
