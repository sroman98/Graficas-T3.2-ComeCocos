using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace ComeCocos
{
    public partial class MainPage : ContentPage
    {
        double xTranslation = 0;
        double yTranslation = 0;
        const double speed = 10;

        const double minSize = 10;
        const double shrinkFactor = 0.8;
        const double growFactor = 1.2;
        double sizeScale = 1;

        readonly Image[] enemies = new Image[5];

        double score;
        public string StringScore
        {
            get { return "Score: " + score; }
            set
            {
                OnPropertyChanged(nameof(StringScore));
            }
        }

        public MainPage()
        {
            InitializeComponent();

            BindingContext = this;
            StringScore = StringScore;

            Device.StartTimer(TimeSpan.FromSeconds(3), () =>
            {
                AddEnemy();
                return true;
            });

            enemies[0] = enemy1;
            enemies[1] = enemy2;
            enemies[2] = enemy3;
            enemies[3] = enemy4;
            enemies[4] = enemy5;
        }

        void LeftButtonCLicked(System.Object sender, System.EventArgs e)
        {
            if (character.X + xTranslation - speed >= 0)
            {
                xTranslation -= speed;
                character.TranslateTo(xTranslation, yTranslation);
                CheckCollisions();
            }
        }

        void UpButtonClicked(System.Object sender, System.EventArgs e)
        {
            if (character.Y + yTranslation - speed >= 0)
            {
                yTranslation -= speed;
                character.TranslateTo(xTranslation, yTranslation);
                CheckCollisions();
            }

        }

        void ShrinkButtonClicked(System.Object sender, System.EventArgs e)
        {
            if(character.Width * sizeScale * shrinkFactor >= minSize)
            {
                sizeScale *= shrinkFactor;
                character.ScaleTo(sizeScale);
            }
        }

        void DownButtonClicked(System.Object sender, System.EventArgs e)
        {
            if (character.Y + character.Height*sizeScale + yTranslation + speed <= canvas.Height)
            {
                yTranslation += speed;
                character.TranslateTo(xTranslation, yTranslation);
                CheckCollisions();
            }
        }

        void RightButtonClicked(object sender, EventArgs e)
        {
            if (character.X + character.Width*sizeScale + xTranslation + speed <= canvas.Width)
            {
                xTranslation += speed;
                character.TranslateTo(xTranslation, yTranslation);
                CheckCollisions();
            }
        }

        private void AddEnemy()
        {
            foreach (Image enemy in enemies)
            {
                if(!enemy.IsVisible)
                {
                    double tx;
                    double ty;

                    Image img = new Image { Source = "enemy", WidthRequest = 30, HeightRequest = 30 };
                    do
                    {
                        double maxWidth = canvas.Width - enemy.Width;
                        double maxHeight = canvas.Height - enemy.Height;
                        
                        tx = new Random().Next((int)(maxWidth + 1));
                        ty = new Random().Next((int)(maxHeight + 1));
                        img.TranslationX = tx;
                        img.TranslationY = ty;
                    } while (AreColliding(character, img));

                    enemy.TranslationX = tx;
                    enemy.TranslationY = ty;
                    enemy.IsVisible = true;
                    CheckCollisions();
                    return;
                }
            }
        }

        private void CheckCollisions()
        {
            foreach(Image enemy in enemies)
            {
                if(enemy.IsVisible && AreColliding(character, enemy))
                {
                    DidCollide(enemy);
                }
            }
        }

        private bool AreColliding(Image character, Image enemy)
        {
            double x1 = character.X + character.TranslationX;
            double x2 = enemy.TranslationX;
            double y1 = character.Y + character.TranslationY;
            double y2 = enemy.TranslationY;

            double size1 = character.Width * sizeScale;

            double d = Math.Sqrt(Math.Pow(x1 - x2, 2) + Math.Pow(y1 - y2, 2));

            if (d < size1 / 2 + enemy.Width / 2)
            {
                return true;
            }
            return false;
        }

        private void DidCollide(Image enemy)
        {
            score += 1;
            StringScore = StringScore;

            enemy.IsVisible = false;
            sizeScale *= growFactor;
            character.ScaleTo(sizeScale);

            double rDir;
            if(new Random().NextDouble() <= 0.5)
            {
                rDir = 1;
            } else
            {
                rDir = -1;
            }

            character.RelRotateTo(360 * rDir);
        }
    }
}
