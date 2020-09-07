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
        private Button scoreTrackerButton;
        private Button courseLookupButton;
        private Button aboutButton;
        private Button sandAstheticButton1;
        private Button sandAstheticButton2;
        private Button waterAstheticButton1;
        private Button waterAstheticButton2;
        private Label hole;
        private Label customCoursePrompt;
        private Entry customCourseEntry;
        private Button customCourseNextButton;
        private Button customNineButton;
        private Button customEighteenButton;
        private NavigationPage np;
        private CirclePage mp;
        private CirclePage sp;
        private CirclePage ssp;
        private CirclePage fp;
        private CirclePage qp;
        private CirclePage ep;
        private AbsoluteLayout homePageLayout;
        private AbsoluteLayout enterPageLayout;
        private CircleScrollView courseSelectionLayout;
        CircleScrollView finalScreenLayout;
        private Button roundInfoButton;
        private Button overallButton;
        private Button strokeButton;
        private StackLayout finalLayout;
        //Current number of strokes for a single hole
        private int strokes;
        //Current course being played (after selecting new game)
        private string currentCourseName;
        private Courses courses;
        //Current hole that the player has navigated to
        private int currentHole;
        //Total strokes for the round
        private int currentCourseScore;
        //Current course score for the furthest hole the player has nagigated to minus the total par score up to that hole
        private int currentCourseScoreRelativeToPar;
        private int[] scoreCard;
        //The furthest hole that the player has navigated to
        private int furthestHole;
        //If this is false, keep the players game data. Allow them to restart or resume if they have already begun a game (=1)
        private bool midRound = false;
        private int nineOrEighteen;
        private Color greenColor = Color.FromRgb(10, 80, 22);
        private Color grayColor = Color.FromRgb(70, 70, 70);
        private Color darkGreenColor = Color.FromRgb(0, 35, 0);
        private Color sandColor = Color.FromRgb(218, 189, 129);
        private Color waterColor = Color.FromRgb(0, 64, 98);
        public App()
        {
            courses = new Courses();

            scoreTrackerButton = new Button() { Text = "Score Tracker", BackgroundColor = greenColor };
            courseLookupButton = new Button() { Text = "Course Lookup", BackgroundColor = greenColor };
            aboutButton = new Button() { Text = "About", FontSize = 4, BackgroundColor = greenColor };
            sandAstheticButton1 = new Button() { Text = "", BackgroundColor = sandColor };
            sandAstheticButton2 = new Button() { Text = "", BackgroundColor = sandColor };
            waterAstheticButton1 = new Button() { Text = "", BackgroundColor = waterColor };
            waterAstheticButton2 = new Button() { Text = "", BackgroundColor = waterColor };
            hole = new Label() { Text = ".", TextColor = Color.Black };

            roundInfoButton = new Button() { Text = "", BackgroundColor = grayColor };
            overallButton = new Button() { Text = "", BackgroundColor = grayColor };
            Button addStrokeButton = new Button() { Text = "+1", FontSize = 20, BackgroundColor = greenColor };
            strokeButton = new Button() { Text = "0", BackgroundColor = greenColor };
            Button subtractStrokeButton = new Button() { Text = "-1", BackgroundColor = greenColor };
            Button nextHoleButton = new Button() { Text = "Next\nHole", FontSize = Device.GetNamedSize(NamedSize.Small, typeof(Label)), BackgroundColor = sandColor, TextColor = Color.Black };
            Button previousHoleButton = new Button() { Text = "Prev\nHole", FontSize = Device.GetNamedSize(NamedSize.Small, typeof(Label)), BackgroundColor = sandColor, TextColor = Color.Black };
            Button resumeGameQuestionButton = new Button() { Text = "Resume Game", BackgroundColor = greenColor };
            Button newGameQuestionButton = new Button() { Text = "New Round", BackgroundColor = greenColor };
            
            
            addStrokeButton.Clicked += OnAddStrokeButtonClicked;
            strokeButton.Clicked += OnStrokeButtonClicked;
            subtractStrokeButton.Clicked += OnSubtractStrokeButtonClicked;
            nextHoleButton.Clicked += OnNextHoleButtonClicked;
            previousHoleButton.Clicked += OnPreviousHoleButtonClicked;
            resumeGameQuestionButton.Clicked += OnResumeGameQuestionButtonClicked;
            newGameQuestionButton.Clicked += OnNewGameQuestionButtonClicked;
            aboutButton.Clicked += OnAboutButtonClicked;
            waterAstheticButton1.Clicked += OnWaterAstheticButtonClicked;
            waterAstheticButton2.Clicked += OnWaterAstheticButtonClicked;

            StackLayout coursesLayout = new CircleStackLayout{};

            StackLayout questionCircleStackLayout = new CircleStackLayout 
            {
                Children = 
                {
                    resumeGameQuestionButton,
                    newGameQuestionButton
                }
            };

            List<string> courseList = courses.GetCourseList();

            int courseNameFontSize = 0;
            for (int i = 0; i<courseList.Count(); i++)
            {
                if (i == 0 || i == courseList.Count() - 1)
                {
                    courseNameFontSize = 8;
                }
                else
                {
                    courseNameFontSize = 10;
                }
                Button courseNameButton = new Button()
                {
                    Text = courseList[i],
                    BackgroundColor = greenColor,
                    FontSize = courseNameFontSize
                };
                courseNameButton.Clicked += EvaluateCustomOrStandardGame;
                coursesLayout.Children.Add(courseNameButton);
            }

            AbsoluteLayout.SetLayoutBounds(roundInfoButton, new Rectangle(0.5, 0, 155, 50));
            AbsoluteLayout.SetLayoutFlags(roundInfoButton, AbsoluteLayoutFlags.PositionProportional);

            AbsoluteLayout.SetLayoutBounds(overallButton, new Rectangle(0.5, 0.17, 155, 50));
            AbsoluteLayout.SetLayoutFlags(overallButton, AbsoluteLayoutFlags.PositionProportional);

            AbsoluteLayout.SetLayoutBounds(addStrokeButton, new Rectangle(0.5, 0.75, 155, 120));
            AbsoluteLayout.SetLayoutFlags(addStrokeButton, AbsoluteLayoutFlags.PositionProportional);

            AbsoluteLayout.SetLayoutBounds(strokeButton, new Rectangle(0.5, 0.367, 80, 60));
            AbsoluteLayout.SetLayoutFlags(strokeButton, AbsoluteLayoutFlags.PositionProportional);

            AbsoluteLayout.SetLayoutBounds(subtractStrokeButton, new Rectangle(0.5, 1, 155, 58));
            AbsoluteLayout.SetLayoutFlags(subtractStrokeButton, AbsoluteLayoutFlags.PositionProportional);

            AbsoluteLayout.SetLayoutBounds(nextHoleButton, new Rectangle(1, 0, 100, 360));
            AbsoluteLayout.SetLayoutFlags(nextHoleButton, AbsoluteLayoutFlags.PositionProportional);

            AbsoluteLayout.SetLayoutBounds(previousHoleButton, new Rectangle(0, 0, 100, 360));
            AbsoluteLayout.SetLayoutFlags(previousHoleButton, AbsoluteLayoutFlags.PositionProportional);

            AbsoluteLayout.SetLayoutBounds(scoreTrackerButton, new Rectangle(0, 0.35, 200, 90));
            AbsoluteLayout.SetLayoutFlags(scoreTrackerButton, AbsoluteLayoutFlags.PositionProportional);

            AbsoluteLayout.SetLayoutBounds(courseLookupButton, new Rectangle(0, 0.65, 200, 70));
            AbsoluteLayout.SetLayoutFlags(courseLookupButton, AbsoluteLayoutFlags.PositionProportional);

            AbsoluteLayout.SetLayoutBounds(aboutButton, new Rectangle(0.8, 0.5, 80, 80));
            AbsoluteLayout.SetLayoutFlags(aboutButton, AbsoluteLayoutFlags.PositionProportional);

            AbsoluteLayout.SetLayoutBounds(sandAstheticButton1, new Rectangle(0.65, 0.3, 40, 40));
            AbsoluteLayout.SetLayoutFlags(sandAstheticButton1, AbsoluteLayoutFlags.PositionProportional);

            AbsoluteLayout.SetLayoutBounds(sandAstheticButton2, new Rectangle(0.8, 0.74, 80, 50));
            AbsoluteLayout.SetLayoutFlags(sandAstheticButton2, AbsoluteLayoutFlags.PositionProportional);

            AbsoluteLayout.SetLayoutBounds(waterAstheticButton1, new Rectangle(0.3, 0, 250, 80));
            AbsoluteLayout.SetLayoutFlags(waterAstheticButton1, AbsoluteLayoutFlags.PositionProportional);

            AbsoluteLayout.SetLayoutBounds(waterAstheticButton2, new Rectangle(0.2, 1, 200, 80));
            AbsoluteLayout.SetLayoutFlags(waterAstheticButton2, AbsoluteLayoutFlags.PositionProportional);

            AbsoluteLayout.SetLayoutBounds(hole, new Rectangle(0.8, .4, 30, 30));
            AbsoluteLayout.SetLayoutFlags(hole, AbsoluteLayoutFlags.PositionProportional);

            homePageLayout = new AbsoluteLayout
            {
                Children =
                {
                    scoreTrackerButton,
                    courseLookupButton,
                    aboutButton,
                    sandAstheticButton1,
                    sandAstheticButton2,
                    waterAstheticButton1,
                    waterAstheticButton2,
                    hole
                }
            };

            courseSelectionLayout = new CircleScrollView
            {
                Content = coursesLayout
            };

            CircleScrollView questionLayout = new CircleScrollView
            {
                Content = questionCircleStackLayout
            };

            AbsoluteLayout parTrackerLayout = new AbsoluteLayout()
            {
                Children =
                {
                    roundInfoButton,
                    overallButton,
                    addStrokeButton,
                    strokeButton,
                    subtractStrokeButton,
                    nextHoleButton,
                    previousHoleButton
                }
            };

            finalLayout = new StackLayout
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

            finalScreenLayout = new CircleScrollView
            {
                Content = finalLayout
            };


            



            //MainPage
            mp = new CirclePage() {
                Content = homePageLayout,
                BackgroundColor = darkGreenColor
            };

            //SubPage
            sp = new CirclePage()
            {
                Content = courseSelectionLayout,
                BackgroundColor = darkGreenColor
            };

            //SubSubPage
            ssp = new CirclePage()
            {
                Content = parTrackerLayout,
                BackgroundColor = darkGreenColor
            };

            //QuestionPage
            qp = new CirclePage() 
            { 
                Content = questionLayout,
                BackgroundColor = darkGreenColor
            };

            //EnterPage
            ep = new CirclePage()
            {
                BackgroundColor = darkGreenColor
            };


            //FinalPage (results screen)
            fp = new CirclePage()
            {
                Content = finalScreenLayout,
                BackgroundColor = darkGreenColor
            };

            NavigationPage np = new NavigationPage(mp);
            NavigationPage.SetHasNavigationBar(mp, false);
            NavigationPage.SetHasNavigationBar(sp, false);
            NavigationPage.SetHasNavigationBar(ssp, false);
            NavigationPage.SetHasNavigationBar(qp, false);
            NavigationPage.SetHasNavigationBar(ep, false);
            NavigationPage.SetHasNavigationBar(fp, false);
            
            MainPage = np;
            scoreTrackerButton.Clicked += DetermineNewOrResumeGame;
            courseLookupButton.Clicked += OnHistoryButtonClicked;
        }
        protected void GoToNamePrompt(object sender, System.EventArgs e)
        {
            nineOrEighteen = Convert.ToInt32((sender as Button).Text);
            customCoursePrompt.Text = "Enter course name:";
            customCourseEntry.Keyboard = Keyboard.Text;
            enterPageLayout.Children.Remove(customNineButton);
            enterPageLayout.Children.Remove(customEighteenButton);
            enterPageLayout.Children.Add(customCourseEntry);
            enterPageLayout.Children.Add(customCourseNextButton);
        }
        protected void GoToParPrompt(object sender, System.EventArgs e)
        {
            if (customCourseNextButton.Text == "Next")
            {
                customCourseNextButton.Text = "Start";
                currentCourseName = customCourseEntry.Text;
                customCourseEntry.Text = "";
                customCourseEntry.Keyboard = Keyboard.Numeric;
                customCourseEntry.MaxLength = nineOrEighteen;
                customCoursePrompt.Text = "Enter course pars in order (Example: 443545344)";
            }
            else if (customCourseNextButton.Text == "Start")
            {
                string pars = customCourseEntry.Text;
                //TODO: Add checking of entry
                //Add course to list of courses
                List<int> customCourseParList = new List<int>();
                for (int i = 0; i < nineOrEighteen; i++)
                {
                    customCourseParList.Add(Convert.ToInt32(pars[i].ToString()));
                }

                int[] customCourseParListIntArray = customCourseParList.ToArray();
                courses.AddNewCourse(currentCourseName, customCourseParListIntArray);

                NewGame(currentCourseName);
                MainPage.Navigation.RemovePage(ep);
            }
        }

        protected async void DetermineNewOrResumeGame(object sender, System.EventArgs e)
        {
            if (midRound)
            {
                //Ask player whether they want to resume or start new
                await MainPage.Navigation.PushAsync(qp);
            }
            else
            {
                //Bring up course selection - it is a new game
                await MainPage.Navigation.PushAsync(sp);
            }
        }
        protected void EvaluateCustomOrStandardGame(object sender, System.EventArgs e)
        {
            string courseName = (sender as Button).Text;
            if (courseName == "Custom Round")
            {
                NewCustomGame();
            }
            else
            {
                nineOrEighteen = courses.GetNineOrEighteen(courseName);
                NewGame(courseName);
            }
        }

        protected async void NewCustomGame()
        {
            //initiate values
            customCoursePrompt = new Label { Text = "9 or 18 holes:" };
            customCourseEntry = new Entry() { };
            customCourseNextButton = new Button() { Text = "Next" };
            customNineButton = new Button() { Text = "9" };
            customEighteenButton = new Button() { Text = "18" };

            enterPageLayout = new AbsoluteLayout()
            {
                Children =
                {
                    customCoursePrompt,
                    customNineButton,
                    customEighteenButton
                }
            };

            AbsoluteLayout.SetLayoutBounds(customCoursePrompt, new Rectangle(0.5, 0.5, 300, 200));
            AbsoluteLayout.SetLayoutFlags(customCoursePrompt, AbsoluteLayoutFlags.PositionProportional);
            AbsoluteLayout.SetLayoutBounds(customCourseEntry, new Rectangle(0.5, 0.7, 300, 60));
            AbsoluteLayout.SetLayoutFlags(customCourseEntry, AbsoluteLayoutFlags.PositionProportional);
            AbsoluteLayout.SetLayoutBounds(customCourseNextButton, new Rectangle(0.5, 0.95, 100, 60));
            AbsoluteLayout.SetLayoutFlags(customCourseNextButton, AbsoluteLayoutFlags.PositionProportional);
            AbsoluteLayout.SetLayoutBounds(customNineButton, new Rectangle(0.2, 0.6, 100, 60));
            AbsoluteLayout.SetLayoutFlags(customNineButton, AbsoluteLayoutFlags.PositionProportional);
            AbsoluteLayout.SetLayoutBounds(customEighteenButton, new Rectangle(0.8, 0.6, 100, 60));
            AbsoluteLayout.SetLayoutFlags(customEighteenButton, AbsoluteLayoutFlags.PositionProportional);

            customCourseNextButton.Clicked += GoToParPrompt;
            customNineButton.Clicked += GoToNamePrompt;
            customEighteenButton.Clicked += GoToNamePrompt;

            ep.Content = enterPageLayout;

            await MainPage.Navigation.PushAsync(ep);
        }

        protected async void NewGame(string courseName)
        {
            //New game - assign values
            midRound = true;
            strokes = 0;
            currentHole = 1;
            currentCourseScore = 0;
            currentCourseScoreRelativeToPar = 0;
            furthestHole = 1;
            currentCourseName = courseName;
            if (nineOrEighteen == 18)
            {
                scoreCard = new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            }
            else
            {
                scoreCard = new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            }

            roundInfoButton.Text = "H" + Convert.ToString(currentHole)+ " P" + Convert.ToString(courses.GetHolePar(currentCourseName, 1));
            overallButton.Text = "ovr: " + Convert.ToString(currentCourseScoreRelativeToPar);
            strokeButton.Text = Convert.ToString(strokes);

            await MainPage.Navigation.PushAsync(ssp);
            MainPage.Navigation.RemovePage(sp);
        }
        protected async void OnResumeGameQuestionButtonClicked(object sender, System.EventArgs e)
        {
            //Keep values
            roundInfoButton.Text = "H" + Convert.ToString(currentHole) + " P" + Convert.ToString(courses.GetHolePar(currentCourseName, currentHole));
            overallButton.Text = "ovr: " + currentCourseScoreRelativeToPar;
            await MainPage.Navigation.PushAsync(ssp);
            MainPage.Navigation.RemovePage(qp);
        }

        protected void OnNewGameQuestionButtonClicked(object sender, System.EventArgs e)
        {
            MainPage.Navigation.PushAsync(sp);
            MainPage.Navigation.RemovePage(qp);
        }

        protected void OnAddStrokeButtonClicked(object sender, System.EventArgs e)
        {
            strokes += 1;
            strokeButton.Text = Convert.ToString(strokes);
        }
        protected void OnStrokeButtonClicked(object sender, System.EventArgs e)
        {
            //For now do nothing, possibly add a prompt to tell the user to select amount
        }
        protected void OnSubtractStrokeButtonClicked(object sender, System.EventArgs e)
        {
            if (strokes == 0)
            {
                return;
            }
            strokes -= 1;
            strokeButton.Text = Convert.ToString(strokes);
        }

        protected void OnHistoryButtonClicked(object sender, System.EventArgs e)
        {
            courseLookupButton.Text = "clicked";
        }

        protected void OnNextHoleButtonClicked(object sender, System.EventArgs e)
        {
            if (strokes == 0)
            {
                Toast.DisplayText("Enter a score for this hole");
                return;
            }
            scoreCard[currentHole - 1] = strokes;
            currentCourseScore = scoreCard.Sum();
            currentCourseScoreRelativeToPar = courses.CalculateCurrentScore(currentCourseName, furthestHole, currentCourseScore);

            if (currentHole == 18 && nineOrEighteen == 18)
            {
                FinishRound();
                return;
            }
            else if (currentHole == 9 && nineOrEighteen == 9)
            {
                FinishRound();
                return;
            }

            //Ensure your relative par is based on the furthest hole that you have visited in the app
            if (currentHole == furthestHole)
            {
                furthestHole += 1;
            }
            currentHole += 1;

            UpdateButtonsNext();
        }

        protected void OnPreviousHoleButtonClicked(object sender, System.EventArgs e)
        {
            if (currentHole == 1)
            {
                return;
            }
            
            if (strokes == 0)
            {
                Toast.DisplayText("Enter a score before returning to previous holes");
                return;
            }
            if (strokes != 0) {
                scoreCard[currentHole - 1] = strokes;
                currentCourseScore = scoreCard.Sum();
                currentCourseScoreRelativeToPar = courses.CalculateCurrentScore(currentCourseName, furthestHole, currentCourseScore);
            }
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
            roundInfoButton.Text = "H" + currentHole + " P" + Convert.ToString(courses.GetHolePar(currentCourseName, currentHole));
            overallButton.Text = "ovr: " + relativeCourseScoreString;
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
            roundInfoButton.Text = "H" + currentHole + " P" + Convert.ToString(courses.GetHolePar(currentCourseName, currentHole));
            overallButton.Text = "ovr: " + relativeCourseScoreString;
            strokes = scoreCard[currentHole - 1];
            strokeButton.Text = Convert.ToString(strokes);
        }

        public void FinishRound()
        {
            finalLayout = new StackLayout
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
            finalScreenLayout.Content = finalLayout;

            finalLayout.Children.Add(new Label
            {
                Text = currentCourseName,
                HorizontalOptions = LayoutOptions.Center,
                HorizontalTextAlignment = TextAlignment.Center
            });

            string relativeCourseScoreString;
            if (currentCourseScoreRelativeToPar > 0)
            {
                relativeCourseScoreString = "+" + Convert.ToString(currentCourseScoreRelativeToPar);
            }
            else
            {
                relativeCourseScoreString = Convert.ToString(currentCourseScoreRelativeToPar);
            }

            finalLayout.Children.Add(new Label
            {
                Text = "Final Score: " + currentCourseScore,
                HorizontalOptions = LayoutOptions.Center,
                HorizontalTextAlignment = TextAlignment.Center
            });

            finalLayout.Children.Add(new Label
            {
                Text = "Overall: " + relativeCourseScoreString,
                HorizontalOptions = LayoutOptions.Center,
                HorizontalTextAlignment = TextAlignment.Center
            });

            for (int i = 0; i < nineOrEighteen; i++)
            {
                finalLayout.Children.Add(new Label
                {
                    Text = "Hole " + Convert.ToString(i+1) + " (" + courses.GetHolePar(currentCourseName, i + 1) + "): " + scoreCard[i],
                    HorizontalOptions = LayoutOptions.Center,
                    HorizontalTextAlignment = TextAlignment.Center
                });
                if (i == 17 && nineOrEighteen == 18)
                {
                    finalLayout.Children.Add(new Label
                    {
                        Text = ""
                    });
                }
                else if (i == 8 && nineOrEighteen == 9)
                {
                    finalLayout.Children.Add(new Label
                    {
                        Text = ""
                    });
                }
            }
            MainPage.BackgroundColor = Color.Yellow;
            MainPage.Navigation.PushAsync(fp);
            MainPage.Navigation.RemovePage(ssp);
            midRound = false;
        }

        protected void OnAboutButtonClicked(object sender, System.EventArgs e)
        {
            Toast.DisplayText("Andrew Johnson         github.com/AndrewAJoh");
        }

        protected void OnWaterAstheticButtonClicked(object sender, System.EventArgs e)
        {
            Toast.DisplayText("Splash!");
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
