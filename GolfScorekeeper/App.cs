using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xamarin.Forms;
using Tizen.Wearable.CircularUI.Forms;
using Tizen.NUI.BaseComponents;

namespace GolfScorekeeper
{
    public class App : Application
    {
        private Button newGameButton;
        private Button courseLookupButton;
        private NavigationPage np;
        private CirclePage mp;
        private CirclePage sp;
        private CirclePage ssp;
        private CirclePage fp;
        private CircleStackLayout homePageLayout;
        private CircleScrollView courseSelectionLayout;
        private Button roundInfoButton;
        private Button strokeButton;
        private StackLayout finalScreenLayout;
        //Current number of strokes for a single hole
        private int strokes = 0;
        //Current course being played (after selecting new game)
        private string currentCourseName;
        private Courses courses;
        //Current hole that the player has navigated to
        private int currentHole = 1;
        //Total strokes for the round
        private int currentCourseScore = 0;
        //Current course score for the furthest hole the player has nagigated to minus the total par score up to that hole
        private int currentCourseScoreRelativeToPar = 0;
        private int[] scoreCard = new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        //The furthest hole that the player has navigated to
        private int furthestHole = 1;
        public App()
        {
            courses = new Courses();

            newGameButton = new Button() { Text = "New Round" };
            courseLookupButton = new Button() { Text = "Course Lookup" };
            roundInfoButton = new Button() { Text = "" };
            Button addStrokeButton = new Button() { Text = "+1" };
            strokeButton = new Button() { Text = "0" };
            Button subtractStrokeButton = new Button() { Text = "-1" };
            Button nextHoleButton = new Button() { Text = "Next Hole" };
            Button previousHoleButton = new Button() { Text = "Prev Hole" };

            addStrokeButton.Clicked += onAddStrokeButtonClicked;
            strokeButton.Clicked += onStrokeButtonClicked;
            subtractStrokeButton.Clicked += onSubtractStrokeButtonClicked;
            nextHoleButton.Clicked += onNextHoleButtonClicked;
            previousHoleButton.Clicked += onPreviousHoleButtonClicked;

            StackLayout coursesLayout = new CircleStackLayout{};

            List<string> courseList = courses.getCourseList();

            for (int i = 0; i<courseList.Count(); i++)
            {
                Button newGameButton = new Button()
                {
                    Text = courseList[i]
                };
                newGameButton.Clicked += onNewGameCourseSelectionButtonClicked;
                coursesLayout.Children.Add(newGameButton);
            }

            AbsoluteLayout.SetLayoutBounds(roundInfoButton, new Rectangle(0.5, 0, 155, 100));
            AbsoluteLayout.SetLayoutFlags(roundInfoButton, AbsoluteLayoutFlags.PositionProportional);

            AbsoluteLayout.SetLayoutBounds(addStrokeButton, new Rectangle(0.5, 0.75, 155, 120));
            AbsoluteLayout.SetLayoutFlags(addStrokeButton, AbsoluteLayoutFlags.PositionProportional);

            AbsoluteLayout.SetLayoutBounds(strokeButton, new Rectangle(0.5, 0.363, 60, 60));
            AbsoluteLayout.SetLayoutFlags(strokeButton, AbsoluteLayoutFlags.PositionProportional);

            AbsoluteLayout.SetLayoutBounds(subtractStrokeButton, new Rectangle(0.5, 1, 155, 58));
            AbsoluteLayout.SetLayoutFlags(subtractStrokeButton, AbsoluteLayoutFlags.PositionProportional);

            AbsoluteLayout.SetLayoutBounds(nextHoleButton, new Rectangle(1, 0, 100, 360));
            AbsoluteLayout.SetLayoutFlags(nextHoleButton, AbsoluteLayoutFlags.PositionProportional);

            AbsoluteLayout.SetLayoutBounds(previousHoleButton, new Rectangle(0, 0, 100, 360));
            AbsoluteLayout.SetLayoutFlags(previousHoleButton, AbsoluteLayoutFlags.PositionProportional);

            homePageLayout = new CircleStackLayout
            {
                Children =
                {
                    newGameButton,
                    courseLookupButton
                }
            };

            courseSelectionLayout = new CircleScrollView
            {
                Content = coursesLayout
            };

            AbsoluteLayout parTrackerLayout = new AbsoluteLayout()
            {
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

            finalScreenLayout = new StackLayout
            {
                HorizontalOptions = LayoutOptions.Center,
                Children =
                    {
                        new Label
                        {
                            Text = ""
                        }
                    }
            };

            //MainPage
            mp = new CirclePage() {
                Content = homePageLayout
            };

            //SubPage
            sp = new CirclePage()
            {
                Content = courseSelectionLayout
            };

            //SubSubPage
            ssp = new CirclePage()
            {
                Content = parTrackerLayout,
            };

            //FinalPage (results screen)
            fp = new CirclePage()
            {
                Content = finalScreenLayout
            };

            NavigationPage np = new NavigationPage(mp);
            NavigationPage.SetHasNavigationBar(mp, false);
            NavigationPage.SetHasNavigationBar(sp, false);
            NavigationPage.SetHasNavigationBar(ssp, false);
            NavigationPage.SetHasNavigationBar(fp, false);

            MainPage = np;
            newGameButton.Clicked += onNewGameButtonClicked;
            courseLookupButton.Clicked += onHistoryButtonClicked;
        }
        protected async void onNewGameButtonClicked(object sender, System.EventArgs e)
        {
            await MainPage.Navigation.PushAsync(sp);
        }
        protected async void onNewGameCourseSelectionButtonClicked(object sender, System.EventArgs e)
        {
            currentCourseName = (sender as Button).Text;
            roundInfoButton.Text = "Hole 1 | Par " + Convert.ToString(courses.getHolePar(currentCourseName, 1)) + " | 0";
            await MainPage.Navigation.PushAsync(ssp);
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
            if (strokes == 0)
            {
                return;
            }
            strokes -= 1;
            strokeButton.Text = Convert.ToString(strokes);
        }

        protected void onHistoryButtonClicked(object sender, System.EventArgs e)
        {
            courseLookupButton.Text = "clicked";
        }

        protected void onNextHoleButtonClicked(object sender, System.EventArgs e)
        {
            if (currentHole == 18)
            {
                FinishRound();
                return;
            }
            scoreCard[currentHole - 1] = strokes;
            currentCourseScore = scoreCard.Sum();
            currentCourseScoreRelativeToPar = courses.CalculateCurrentScore(currentCourseName, furthestHole, currentCourseScore);

            //Ensure your relative par is based on the furthest hole that you have visited in the app
            if (currentHole == furthestHole)
            {
                furthestHole += 1;
            }
            currentHole += 1;
            
            UpdateButtonsNext();
        }

        protected void onPreviousHoleButtonClicked(object sender, System.EventArgs e)
        {
            if (currentHole == 1)
            {
                return;
            }
            scoreCard[currentHole - 1] = strokes;
            currentCourseScore = scoreCard.Sum();
            currentCourseScoreRelativeToPar = courses.CalculateCurrentScore(currentCourseName, furthestHole, currentCourseScore);
            currentHole -= 1;

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
            roundInfoButton.Text = "Hole " + currentHole + " | Par " + Convert.ToString(courses.getHolePar(currentCourseName, currentHole)) + " | " + relativeCourseScoreString;
            strokes = scoreCard[currentHole - 1];
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
            roundInfoButton.Text = "Hole " + currentHole + " | Par " + Convert.ToString(courses.getHolePar(currentCourseName, currentHole)) + " | " + relativeCourseScoreString;
            strokes = scoreCard[currentHole - 1];
            strokeButton.Text = Convert.ToString(strokes);
        }

        public void FinishRound()
        { 
            //Todo: remove child labels from previous game every time the player plays a round
            Label l = (Label) finalScreenLayout.Children.First();
            l.Text = currentCourseName;

            for (int i = 0; i < 18; i++)
            {
                finalScreenLayout.Children.Add(new Label
                {
                    Text = Convert.ToString(i) + ": " + courses.getHolePar(currentCourseName, i + 1) + " | " + scoreCard[i]
                });
            }
            MainPage.BackgroundColor = Color.Yellow;
            MainPage.Navigation.PushAsync(fp);
            MainPage.Navigation.RemovePage(sp);
            MainPage.Navigation.RemovePage(ssp);
            //Reset for the next round
            currentHole = 1;
            furthestHole = 1;
            currentCourseScore = 0;
            currentCourseScoreRelativeToPar = 0;
            for (int i = 0; i < 18; i++)
            {
                scoreCard[i] = 0;
            }
            currentCourseName = "";
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
