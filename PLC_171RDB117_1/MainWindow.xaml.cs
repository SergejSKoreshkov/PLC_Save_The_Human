using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Windows.Media.Animation;

namespace PLC_171RDB117_1
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Random random = new Random();
        DispatcherTimer enemyTimer = new DispatcherTimer();
        DispatcherTimer targetTimer = new DispatcherTimer();
        bool humanCaptured;

        public MainWindow()
        {
            InitializeComponent();

            enemyTimer.Tick += enemyTimer_Tick;
            enemyTimer.Interval = TimeSpan.FromSeconds(0.5);

            targetTimer.Tick += targetTimer_Tick;
            targetTimer.Interval = TimeSpan.FromSeconds(0.5);
        }

        private void targetTimer_Tick(object sender, EventArgs e)
        {
            progressBarTimeToEnd.Value += 1;
            if (progressBarTimeToEnd.Value >= progressBarTimeToEnd.Maximum)
            {
                EndGame();
            }
        }

        private void EndGame()
        {
           if (!canvasPlayArea.Children.Contains(this.txtGameOver))
            {
                enemyTimer.Stop();
                targetTimer.Stop();
                humanCaptured = false;
                btnStart.Visibility = Visibility.Visible;
                canvasPlayArea.Children.Add(txtGameOver);
                txtGameOver.Text = "Score: " + this.level.Text + "\nGame Over";
            }
            this.level.Text = "0";
        }

        private void enemyTimer_Tick(object sender, EventArgs e)
        {
            addEnemy();
        }

        private void addEnemy()
        {
            ContentControl enemy = new ContentControl();
            enemy.Template = Resources["EnemyTemplate"] as ControlTemplate;
            animateEnemy(enemy, 0, canvasPlayArea.ActualWidth - 100, "(Canvas.Left)");
            animateEnemy(enemy, random.Next((int)canvasPlayArea.ActualHeight - 100), random.Next((int)canvasPlayArea.ActualHeight - 100), "(Canvas.Top)");
            canvasPlayArea.Children.Add(enemy);
            enemy.MouseEnter += enemy_MouseEnter;
        }

        private void enemy_MouseEnter(object sender, MouseEventArgs e)
        {
            if (humanCaptured)
            {
                EndGame();
            }
        }

        private void animateEnemy(ContentControl enemy, int from, double to, string prop)
        {
            Storyboard sb = new Storyboard() { AutoReverse = true, RepeatBehavior = RepeatBehavior.Forever };
            DoubleAnimation animation = new DoubleAnimation()
            {
                From = from,
                To = to,
                Duration = new Duration(TimeSpan.FromSeconds(random.Next(4, 6)))
            };

            Storyboard.SetTarget(animation, enemy);
            PropertyPath path = new PropertyPath(prop);
            Storyboard.SetTargetProperty(animation, path);
            sb.Children.Add(animation);
            sb.Begin();
        }

        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            StartGame();
        }

        private void StartGame()
        {
            figHuman.IsHitTestVisible = true;
            humanCaptured = false;
            progressBarTimeToEnd.Value = 0;
            btnStart.Visibility = Visibility.Visible;
            canvasPlayArea.Children.Clear();
            canvasPlayArea.Children.Add(figTarger);
            canvasPlayArea.Children.Add(figHuman);
            enemyTimer.Start();
            targetTimer.Start();
        }

        private void figHuman_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (enemyTimer.IsEnabled) {
                humanCaptured = true;
                figHuman.IsHitTestVisible = false;
            }
        }

        private void canvasPlayArea_MouseMove(object sender, MouseEventArgs e)
        {
            if (humanCaptured)
            {
                Point pointerPosition = e.GetPosition(null);
                Point relativePosition = grid.TransformToVisual(canvasPlayArea).Transform(pointerPosition);
                if ((Math.Abs(relativePosition.X - Canvas.GetLeft(figHuman)) > figHuman.ActualWidth * 2) ||
                    (Math.Abs(relativePosition.Y - Canvas.GetTop(figHuman)) > figHuman.ActualHeight * 2))
                {
                    humanCaptured = false;
                    figHuman.IsHitTestVisible = true;
                } else
                {
                    Canvas.SetLeft(figHuman, relativePosition.X - figHuman.ActualWidth / 2);
                    Canvas.SetTop(figHuman, relativePosition.Y - figHuman.ActualHeight / 2);;
                }
            }
        }

        private void figTarger_MouseEnter(object sender, MouseEventArgs e)
        {
            if (targetTimer.IsEnabled && humanCaptured)
            {
                Canvas.SetLeft(figTarger, random.Next(100, (int)canvasPlayArea.ActualWidth - 100));
                Canvas.SetTop(figTarger, random.Next(100, (int)canvasPlayArea.ActualHeight - 100));
                Canvas.SetLeft(figHuman, random.Next(100, (int)canvasPlayArea.ActualWidth - 100));
                Canvas.SetTop(figHuman, random.Next(100, (int)canvasPlayArea.ActualHeight - 100));
                humanCaptured = false;
                figHuman.IsHitTestVisible = true;
                progressBarTimeToEnd.Value = 0;
                this.level.Text = (Int16.Parse(this.level.Text) + 1).ToString();
            }
        }
    }
}
