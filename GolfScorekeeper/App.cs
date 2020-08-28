using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xamarin.Forms;
using Tizen.Wearable.CircularUI.Forms;

namespace GolfScorekeeper
{
    public class App : Application
    {
        private Button newGameButton;
        private Button historyButton;
        private NavigationPage np;
        private CirclePage p;
        private CirclePage cp;
        private CirclePage cpp;
        private CircleScrollView sv;
        private CircleScrollView sv2;
        private CircleScrollView sv3;
        private int strokes = 0;
        private Button roundInfoButton;
        private Button strokeButton;
        private string currentCourseName;
        private Courses courses;
        private int currentHoleCount = 1;
        private int currentCourseScore = 0;
        private int currentCourseScoreRelativeToPar = 0;
        private int[] scoreCard = new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        private int furthestHole = 1;
        public App()
        {
            courses = new Courses();
            newGameButton = new Button() { Text = "New Round" };
            historyButton = new Button() { Text = "Previous Scores" };

            sv = new CircleScrollView
            {

                Content = new StackLayout
                {

                    Orientation = StackOrientation.Vertical,
                    Children =
                    {
                        newGameButton,
                        historyButton
                    }
                }
            };

            StackLayout coursesLayout = new StackLayout
            {

                Orientation = StackOrientation.Vertical,
                Children =
                    {
                        new Label
                        {
                            Text = "welcome to the 2nd page"
                        }
                    }
            };

            List<string> courseList = courses.getCourseList();
            //Currently unused but could be useful potentially
            List<Button> buttonList = new List<Button>();

            for (int i = 0; i<courseList.Count(); i++)
            {
                Button newGameButton = new Button()
                {
                    Text = courseList[i]
                };
                newGameButton.Clicked += onNewGameCourseSelectionButtonClicked;
                buttonList.Add(newGameButton);
                coursesLayout.Children.Add(newGameButton);
            }

            roundInfoButton = new Button()
            {
                Text = ""
            };

            Button addStrokeButton = new Button()
            {
                Text = "+1"
            };

            strokeButton = new Button()
            {
                Text = ""
            };

            Button subtractStrokeButton = new Button()
            {
                Text = "-1"
            };

            Button nextHoleButton = new Button()
            {
                Text = "Next Hole"
            };

            Button previousHoleButton = new Button()
            {
                Text = "Previous Hole"
            };

            addStrokeButton.Clicked += onAddStrokeButtonClicked;
            strokeButton.Clicked += onStrokeButtonClicked;
            subtractStrokeButton.Clicked += onSubtractStrokeButtonClicked;
            nextHoleButton.Clicked += onNextHoleButtonClicked;
            previousHoleButton.Clicked += onPreviousHoleButtonClicked;

            StackLayout parTracker = new StackLayout
            {
                Orientation = StackOrientation.Vertical,
                Children =
                    {
                        roundInfoButton,
                        addStrokeButton,
                        strokeButton,
                        subtractStrokeButton,
                        nextHoleButton,
                        previousHoleButton
                    }
            };


            sv2 = new CircleScrollView
            {
                Content = coursesLayout
            };

            sv3 = new CircleScrollView
            {
                Content = parTracker
            };

            p = new CirclePage();
            p.RotaryFocusObject = sv;
            p.Content = sv;

            cp = new CirclePage()
            {
                Content = sv2
            };
            cp.RotaryFocusObject = sv2;
            cp.Content = sv2;

            cpp = new CirclePage()
            {
                Content = sv3
            };
            cpp.RotaryFocusObject = sv3;
            cpp.Content = sv3;

            NavigationPage np = new NavigationPage(p);
            MainPage = np;
            newGameButton.Clicked += onNewGameButtonClicked;
            historyButton.Clicked += onHistoryButtonClicked;
        }
        protected async void onNewGameButtonClicked(object sender, System.EventArgs e)
        {
            await MainPage.Navigation.PushAsync(cp);
        }
        protected async void onNewGameCourseSelectionButtonClicked(object sender, System.EventArgs e)
        {
            currentCourseName = (sender as Button).Text;
            roundInfoButton.Text = "Hole 1 | Par " + Convert.ToString(courses.getHolePar(currentCourseName, 1)) + " | 0";
            await MainPage.Navigation.PushAsync(cpp);
        }
        protected void onAddStrokeButtonClicked(object sender, System.EventArgs e)
        {
            strokes += 1;
            strokeButton.Text = Convert.ToString(strokes);
        }
        protected void onStrokeButtonClicked(object sender, System.EventArgs e)
        {
            //For now do nothing, possibly add a prompt to tell the user to select amount
        }
        protected void onSubtractStrokeButtonClicked(object sender, System.EventArgs e)
        {
            strokes -= 1;
            strokeButton.Text = Convert.ToString(strokes);
        }

        protected void onHistoryButtonClicked(object sender, System.EventArgs e)
        {
            historyButton.Text = "clicked";
        }

        protected void onNextHoleButtonClicked(object sender, System.EventArgs e)
        {
            scoreCard[currentHoleCount - 1] = strokes;
            currentCourseScore = scoreCard.Sum();
            currentCourseScoreRelativeToPar = courses.CalculateCurrentScore(currentCourseName, furthestHole, currentCourseScore);

            //Ensure your relative par is based on the furthest hole that you have visited in the app
            if (currentHoleCount == furthestHole)
            {
                furthestHole += 1;
            }
            currentHoleCount += 1;
            
            UpdateButtonsNext();
        }

        protected void onPreviousHoleButtonClicked(object sender, System.EventArgs e)
        {
            if (currentHoleCount == 1)
            {
                return;
            }
            scoreCard[currentHoleCount - 1] = strokes;
            currentCourseScore = scoreCard.Sum();
            currentCourseScoreRelativeToPar = courses.CalculateCurrentScore(currentCourseName, furthestHole, currentCourseScore);
            currentHoleCount -= 1;

            UpdateButtonsPrevious();
        }

        protected void UpdateButtonsNext()
        {
            string relativeCourseScoreString = "";
            if (currentCourseScoreRelativeToPar > 0)
            {
                relativeCourseScoreString = "+" + Convert.ToString(currentCourseScoreRelativeToPar);
            }
            else
            {
                relativeCourseScoreString = Convert.ToString(currentCourseScoreRelativeToPar);
            }
            roundInfoButton.Text = "Hole " + currentHoleCount + " | Par " + Convert.ToString(courses.getHolePar(currentCourseName, currentHoleCount)) + " | " + relativeCourseScoreString;
            strokes = scoreCard[currentHoleCount - 1];
            strokeButton.Text = Convert.ToString(strokes);
        }

        protected void UpdateButtonsPrevious()
        {
            string relativeCourseScoreString = "";
            if (currentCourseScoreRelativeToPar > 0)
            {
                relativeCourseScoreString = "+" + Convert.ToString(currentCourseScoreRelativeToPar);
            }
            else
            {
                relativeCourseScoreString = Convert.ToString(currentCourseScoreRelativeToPar);
            }
            roundInfoButton.Text = "Hole " + currentHoleCount + " | Par " + Convert.ToString(courses.getHolePar(currentCourseName, currentHoleCount)) + " | " + relativeCourseScoreString;
            strokes = scoreCard[currentHoleCount - 1];
            strokeButton.Text = Convert.ToString(strokes);
        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
