using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xamarin.Forms;
using Tizen.Wearable.CircularUI.Forms;
using Tizen.NUI.BaseComponents;
using System.Text.RegularExpressions;
using SQLite;
using SQLitePCL;
using System.IO;

namespace GolfScorekeeper
{
    public class App : Application
    {
        private SQLiteConnection dbConnection;
        private Button scoreTrackerButton;
        private Button courseLookupButton;
        private Button aboutButton;
        private Button teeBoxButton;
        private Button sandAstheticButton1;
        private Button sandAstheticButton2;
        private Button sandAstheticButton3;
        private Button waterAstheticButton1;
        private Button waterAstheticButton2;
        private Label hole;
        private Label redTeeBoxMarker1;
        private Label redTeeBoxMarker2;
        private Label whiteTeeBoxMarker1;
        private Label whiteTeeBoxMarker2;
        private Label areYouSureLabel;
        private Label areYouReallySureLabel;
        private Label customCoursePrompt;
        private Entry customCourseEntry;
        private Button customCourseNextButton;
        private Button customNineButton;
        private Button customEighteenButton;
        private CirclePage mp;
        private CirclePage sp;
        private CirclePage ssp;
        private CirclePage fp;
        private CirclePage qp;
        private CirclePage ep;
        private CirclePage clp;
        private CirclePage cdp;
        private CirclePage dp;
        private AbsoluteLayout homePageLayout;
        private StackLayout coursesLayout;
        private AbsoluteLayout enterPageLayout;
        private AbsoluteLayout courseDetailLayout;
        private CircleScrollView courseSelectionLayout;
        CircleScrollView finalScreenLayout;
        private Button roundInfoButton;
        private Button overallButton;
        private Button strokeButton;
        private StackLayout finalLayout;
        private string courseNameText;
        private List<string> courseList;
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
        private Color puttingGreenColor = Color.FromRgb(84, 161, 88);
        private Color grayColor = Color.FromRgb(70, 70, 70);
        private Color darkGreenColor = Color.FromRgb(0, 35, 0);
        private Color sandColor = Color.FromRgb(218, 189, 129);
        private Color waterColor = Color.FromRgb(0, 64, 98);
        public App()
        {
            courses = new Courses();

            //Get info from database
            raw.SetProvider(new SQLite3Provider_sqlite3());
            raw.FreezeProvider(true);

            string dataPath = global::Tizen.Applications.Application.Current.DirectoryInfo.Data;
            string courseDatabaseFileName = "courses2.db3";
            string courseDatabasePath = Path.Combine(dataPath, courseDatabaseFileName);

            dbConnection = new SQLiteConnection(courseDatabasePath);
            dbConnection.CreateTable<CurrentGame>();
            dbConnection.CreateTable<Course>();

            var courseList = dbConnection.Table<Course>();
            var currentGameList = dbConnection.Table<CurrentGame>();

            List<int> courseParList = new List<int>();

            foreach (var item in courseList)
            {
                courseParList.Clear();
                for (int i = 0; i < item.ParList.Length; i++)
                {
                    courseParList.Add(Convert.ToInt32(item.ParList[i].ToString()));
                }

                int[] courseParListIntArray = courseParList.ToArray();
                courses.AddNewCourse(item.Name, courseParListIntArray);
            }

            var currentGame = 0;
            foreach (var item in currentGameList)
            {
                currentGame++;
            }

            if (currentGame == 1)
            {
                foreach (var gameRecord in currentGameList)
                {
                    midRound = true;
                    currentCourseName = gameRecord.CourseName;
                    currentHole = gameRecord.CurrentHole;
                    furthestHole = gameRecord.FurthestHole;
                    currentCourseScore = gameRecord.CurrentCourseScore;
                    nineOrEighteen = courses.GetNineOrEighteen(currentCourseName);
                    if (nineOrEighteen == 18)
                    {
                        scoreCard = new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
                    }
                    else
                    {
                        scoreCard = new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0 };
                    }
                    for (int i = 0; i < gameRecord.Scorecard.Length; i++)
                    {
                        scoreCard[i] = Convert.ToInt32(Convert.ToString(gameRecord.Scorecard[i]));
                    }
                    strokes = gameRecord.Strokes;

                    if ((currentHole == furthestHole) && (scoreCard[furthestHole - 1] == 0) && (furthestHole != 1)){
                        currentCourseScoreRelativeToPar = courses.CalculateCurrentScore(currentCourseName, furthestHole-1, currentCourseScore);
                    }
                    else if ((currentHole == furthestHole) && (scoreCard[furthestHole - 1] == 0) && (furthestHole == 1))
                    {
                        currentCourseScoreRelativeToPar = 0;
                    }
                    else { 
                        currentCourseScoreRelativeToPar = courses.CalculateCurrentScore(currentCourseName, furthestHole, currentCourseScore);
                    }
                }
            }

            scoreTrackerButton = new Button() { Text = "Score Tracker", BackgroundColor = greenColor };
            courseLookupButton = new Button() { Text = "Course Lookup", BackgroundColor = greenColor };
            aboutButton = new Button() { Text = "About", FontSize = 4, BackgroundColor = puttingGreenColor };
            teeBoxButton = new Button() { BackgroundColor = greenColor };
            sandAstheticButton1 = new Button() { Text = "", BackgroundColor = sandColor };
            sandAstheticButton2 = new Button() { Text = "", BackgroundColor = sandColor };
            sandAstheticButton3 = new Button() { Text = "", BackgroundColor = sandColor };
            waterAstheticButton1 = new Button() { Text = "", BackgroundColor = waterColor };
            waterAstheticButton2 = new Button() { Text = "", BackgroundColor = waterColor };
            hole = new Label() { Text = ".", TextColor = Color.Black };
            redTeeBoxMarker1 = new Label() { Text = ".", TextColor = Color.Red };
            redTeeBoxMarker2 = new Label() { Text = ".", TextColor = Color.Red };
            whiteTeeBoxMarker1 = new Label() { Text = ".", TextColor = Color.White };
            whiteTeeBoxMarker2 = new Label() { Text = ".", TextColor = Color.White };

            roundInfoButton = new Button() { Text = "", BackgroundColor = grayColor };
            overallButton = new Button() { Text = "", BackgroundColor = grayColor };
            Button addStrokeButton = new Button() { Text = "+1", FontSize = 20, BackgroundColor = greenColor };
            strokeButton = new Button() { Text = "0", BackgroundColor = greenColor };
            Button subtractStrokeButton = new Button() { Text = "-1", BackgroundColor = greenColor };
            Button nextHoleButton = new Button() { Text = "Next\nHole", FontSize = Device.GetNamedSize(NamedSize.Small, typeof(Label)), BackgroundColor = sandColor, TextColor = Color.Black };
            Button previousHoleButton = new Button() { Text = "Prev\nHole", FontSize = Device.GetNamedSize(NamedSize.Small, typeof(Label)), BackgroundColor = sandColor, TextColor = Color.Black };
            Button resumeGameQuestionButton = new Button() { Text = "Resume Game", BackgroundColor = greenColor };
            Button newGameQuestionButton = new Button() { Text = "New Round", BackgroundColor = Color.DarkRed };

            areYouSureLabel = new Label() { };
            areYouReallySureLabel = new Label() { FontSize = 8 };
            Button yesButton = new Button() { Text = "Yes", BackgroundColor = greenColor };
            Button noButton = new Button() { Text = "No", BackgroundColor = Color.DarkRed };


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

            AbsoluteLayout deleteCourseLayout = new AbsoluteLayout
            {
                Children =
                {
                    areYouSureLabel,
                    yesButton,
                    noButton,
                    areYouReallySureLabel
                }
            };

            AbsoluteLayout.SetLayoutBounds(areYouSureLabel, new Rectangle(0.5, .25, 250, 80));
            AbsoluteLayout.SetLayoutFlags(areYouSureLabel, AbsoluteLayoutFlags.PositionProportional);

            AbsoluteLayout.SetLayoutBounds(areYouReallySureLabel, new Rectangle(0.5, .50, 250, 80));
            AbsoluteLayout.SetLayoutFlags(areYouReallySureLabel, AbsoluteLayoutFlags.PositionProportional);

            AbsoluteLayout.SetLayoutBounds(yesButton, new Rectangle(0.3, .75, 100, 60));
            AbsoluteLayout.SetLayoutFlags(yesButton, AbsoluteLayoutFlags.PositionProportional);

            AbsoluteLayout.SetLayoutBounds(noButton, new Rectangle(0.7, .75, 100, 60));
            AbsoluteLayout.SetLayoutFlags(noButton, AbsoluteLayoutFlags.PositionProportional);

            yesButton.Clicked += OnYesDeleteButtonClicked;
            noButton.Clicked += OnNoDeleteButtonClicked;

            AbsoluteLayout questionCircleStackLayout = new AbsoluteLayout
            {
                Children =
                {
                    resumeGameQuestionButton,
                    newGameQuestionButton
                }
            };

            AbsoluteLayout.SetLayoutBounds(resumeGameQuestionButton, new Rectangle(0.5, .25, 250, 80));
            AbsoluteLayout.SetLayoutFlags(resumeGameQuestionButton, AbsoluteLayoutFlags.PositionProportional);

            AbsoluteLayout.SetLayoutBounds(newGameQuestionButton, new Rectangle(0.5, .75, 250, 80));
            AbsoluteLayout.SetLayoutFlags(newGameQuestionButton, AbsoluteLayoutFlags.PositionProportional);

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

            AbsoluteLayout.SetLayoutBounds(scoreTrackerButton, new Rectangle(0.1, 0.35, 200, 80));
            AbsoluteLayout.SetLayoutFlags(scoreTrackerButton, AbsoluteLayoutFlags.PositionProportional);

            AbsoluteLayout.SetLayoutBounds(courseLookupButton, new Rectangle(0.1, 0.7, 200, 80));
            AbsoluteLayout.SetLayoutFlags(courseLookupButton, AbsoluteLayoutFlags.PositionProportional);

            AbsoluteLayout.SetLayoutBounds(aboutButton, new Rectangle(0.88, 0.35, 75, 75));
            AbsoluteLayout.SetLayoutFlags(aboutButton, AbsoluteLayoutFlags.PositionProportional);

            AbsoluteLayout.SetLayoutBounds(teeBoxButton, new Rectangle(0.81, 0.69, 60, 50));
            AbsoluteLayout.SetLayoutFlags(teeBoxButton, AbsoluteLayoutFlags.PositionProportional);

            AbsoluteLayout.SetLayoutBounds(sandAstheticButton1, new Rectangle(0.65, 0.5, 25, 25));
            AbsoluteLayout.SetLayoutFlags(sandAstheticButton1, AbsoluteLayoutFlags.PositionProportional);

            AbsoluteLayout.SetLayoutBounds(sandAstheticButton2, new Rectangle(0.1, 0.92, 60, 40));
            AbsoluteLayout.SetLayoutFlags(sandAstheticButton2, AbsoluteLayoutFlags.PositionProportional);

            AbsoluteLayout.SetLayoutBounds(sandAstheticButton3, new Rectangle(0.65, 0.28, 25, 25));
            AbsoluteLayout.SetLayoutFlags(sandAstheticButton3, AbsoluteLayoutFlags.PositionProportional);

            AbsoluteLayout.SetLayoutBounds(waterAstheticButton1, new Rectangle(0.3, 0, 250, 80));
            AbsoluteLayout.SetLayoutFlags(waterAstheticButton1, AbsoluteLayoutFlags.PositionProportional);

            AbsoluteLayout.SetLayoutBounds(waterAstheticButton2, new Rectangle(0.8, 1, 200, 70));
            AbsoluteLayout.SetLayoutFlags(waterAstheticButton2, AbsoluteLayoutFlags.PositionProportional);

            AbsoluteLayout.SetLayoutBounds(hole, new Rectangle(0.85, .4, 30, 30));
            AbsoluteLayout.SetLayoutFlags(hole, AbsoluteLayoutFlags.PositionProportional);

            AbsoluteLayout.SetLayoutBounds(redTeeBoxMarker1, new Rectangle(0.78, .62, 30, 30));
            AbsoluteLayout.SetLayoutFlags(redTeeBoxMarker1, AbsoluteLayoutFlags.PositionProportional);

            AbsoluteLayout.SetLayoutBounds(redTeeBoxMarker2, new Rectangle(0.78, .67, 30, 30));
            AbsoluteLayout.SetLayoutFlags(redTeeBoxMarker2, AbsoluteLayoutFlags.PositionProportional);

            AbsoluteLayout.SetLayoutBounds(whiteTeeBoxMarker1, new Rectangle(0.86, .62, 30, 30));
            AbsoluteLayout.SetLayoutFlags(whiteTeeBoxMarker1, AbsoluteLayoutFlags.PositionProportional);

            AbsoluteLayout.SetLayoutBounds(whiteTeeBoxMarker2, new Rectangle(0.86, .67, 30, 30));
            AbsoluteLayout.SetLayoutFlags(whiteTeeBoxMarker2, AbsoluteLayoutFlags.PositionProportional);

            homePageLayout = new AbsoluteLayout
            {
                Children =
                {
                    scoreTrackerButton,
                    courseLookupButton,
                    aboutButton,
                    teeBoxButton,
                    sandAstheticButton1,
                    sandAstheticButton2,
                    sandAstheticButton3,
                    waterAstheticButton1,
                    waterAstheticButton2,
                    hole,
                    redTeeBoxMarker1,
                    redTeeBoxMarker2,
                    whiteTeeBoxMarker1,
                    whiteTeeBoxMarker2
                }
            };

            courseSelectionLayout = new CircleScrollView { };

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

            //CourseListPage
            clp = new CirclePage()
            {
                BackgroundColor = darkGreenColor
            };

            //CourseDetailPage
            cdp = new CirclePage()
            {
                BackgroundColor = darkGreenColor
            };

            //DeletePage
            dp = new CirclePage()
            {
                Content = deleteCourseLayout,
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
            NavigationPage.SetHasNavigationBar(clp, false);
            NavigationPage.SetHasNavigationBar(cdp, false);
            NavigationPage.SetHasNavigationBar(dp, false);

            MainPage = np;
            scoreTrackerButton.Clicked += DetermineNewOrResumeGame;
            courseLookupButton.Clicked += OnCourseListButtonClicked;
        }
        //Ask for course name. Happens before GoToParPrompt
        protected void GoToNamePrompt(object sender, System.EventArgs e)
        {
            nineOrEighteen = Convert.ToInt32((sender as Button).Text);
            customCoursePrompt.Text = "Enter course name:";
            customCourseEntry.Keyboard = Keyboard.Text;
            customCourseEntry.MaxLength = 25;
            enterPageLayout.Children.Remove(customNineButton);
            enterPageLayout.Children.Remove(customEighteenButton);
            enterPageLayout.Children.Add(customCourseEntry);
            enterPageLayout.Children.Add(customCourseNextButton);
        }
        //Ask for course pars. Rejects if course name is invalid.
        protected void GoToParPrompt(object sender, System.EventArgs e)
        {
            if (customCourseNextButton.Text == "Next")
            {
                var courseNameRegex = new Regex(@"^[a-zA-Z\s]*$");

                if (!courseNameRegex.IsMatch(customCourseEntry.Text))
                {
                    Toast.DisplayText("Only letters and spaces are allowed.");
                    return;
                }

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
                if (pars.Length != nineOrEighteen)
                {
                    Toast.DisplayText("You must have " + nineOrEighteen + " pars in the entry. Follow the example for formatting.");
                    return;
                }

                var parRegex = new Regex("^[1-9]*$");

                if (!parRegex.IsMatch(pars))
                {
                    Toast.DisplayText("0s and symbols are not allowed.");
                    return;
                }

                //Add course to list of courses
                List<int> customCourseParList = new List<int>();
                for (int i = 0; i < nineOrEighteen; i++)
                {
                    customCourseParList.Add(Convert.ToInt32(pars[i].ToString()));
                }

                int[] customCourseParListIntArray = customCourseParList.ToArray();
                int result = courses.AddNewCourse(currentCourseName, customCourseParListIntArray);

                Course c = new Course
                {
                    Name = currentCourseName,
                    ParList = pars
                };
                dbConnection.InsertOrReplace(c);

                GenerateCourseList(false);
                MainPage.Navigation.RemovePage(ep);
                if (result == 1)
                {
                    Toast.DisplayText("Current course information for " + currentCourseName + " has been overwritten.");
                }
            }
        }

        protected void DetermineNewOrResumeGame(object sender, System.EventArgs e)
        {
            if (midRound)
            {
                //Ask player whether they want to resume or start new
                MainPage.Navigation.PushAsync(qp);
            }
            else
            {
                //Bring up course selection - it is a new game
                GenerateCourseList(false);
                MainPage.Navigation.PushAsync(sp);
            }
        }
        protected void EvaluateCustomOrStandardGame(object sender, System.EventArgs e)
        {
            string courseName = (sender as Button).Text;
            if (courseName == "Add Course")
            {
                AddNewCourse();
            }
            else
            {
                nineOrEighteen = courses.GetNineOrEighteen(courseName);
                NewGame(courseName);
            }
        }

        protected async void AddNewCourse()
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
            AbsoluteLayout.SetLayoutBounds(customCourseNextButton, new Rectangle(0.5, 0.95, 110, 60));
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

            roundInfoButton.Text = "H" + Convert.ToString(currentHole) + " P" + Convert.ToString(courses.GetHolePar(currentCourseName, 1));
            if (currentCourseScoreRelativeToPar > 0)
            {
                overallButton.Text = "ovr: +" + Convert.ToString(currentCourseScoreRelativeToPar);
            }
            else 
            {
                overallButton.Text = "ovr: " + Convert.ToString(currentCourseScoreRelativeToPar);
            }
            strokeButton.Text = Convert.ToString(strokes);

            await MainPage.Navigation.PushAsync(ssp);
            MainPage.Navigation.RemovePage(sp);
        }
        protected async void OnResumeGameQuestionButtonClicked(object sender, System.EventArgs e)
        {
            //Keep values
            roundInfoButton.Text = "H" + Convert.ToString(currentHole) + " P" + Convert.ToString(courses.GetHolePar(currentCourseName, currentHole));

            if (currentCourseScoreRelativeToPar > 0)
            {
                overallButton.Text = "ovr: +" + Convert.ToString(currentCourseScoreRelativeToPar);
            }
            else
            {
                overallButton.Text = "ovr: " + Convert.ToString(currentCourseScoreRelativeToPar);
            }
            strokeButton.Text = Convert.ToString(strokes);
            await MainPage.Navigation.PushAsync(ssp);
            MainPage.Navigation.RemovePage(qp);
        }

        protected void OnNewGameQuestionButtonClicked(object sender, System.EventArgs e)
        {
            //Bug fix where new custom course wouldn't show up if mid-round for that course the first time
            GenerateCourseList(false);
            MainPage.Navigation.PushAsync(sp);
            midRound = false;

            //Reset CurrentGame table
            var currentGameQueryResult = dbConnection.Query<CurrentGame>("select * from CurrentGame").FirstOrDefault();
            if (currentGameQueryResult != null)
            {
                dbConnection.RunInTransaction(() =>
                {
                    dbConnection.Delete(currentGameQueryResult);
                });
            }

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

        protected void OnCourseListButtonClicked(object sender, System.EventArgs e)
        {
            if (courses.GetCourseCount() == 0)
            {
                Toast.DisplayText("You have not added any courses yet. Go to 'Score Tracker' -> 'Add Course'.");
                return;
            }
            GenerateCourseList(true);
            MainPage.Navigation.PushAsync(clp);

        }

        protected void GenerateCourseList(bool courseLookupPage)
        {
            coursesLayout = new CircleStackLayout { };
            courseSelectionLayout = new CircleScrollView
            {
                Content = coursesLayout
            };
            if (courseLookupPage) {
                clp.Content = courseSelectionLayout;
            }
            else
            {
                sp.Content = courseSelectionLayout;
            }

            courseList = courses.GetCourseList();

            //Add "Add Course" button as the first option
            if (!courseLookupPage)
            {
                Button addNewCourseButton = new Button
                {
                    Text = "Add Course",
                    BackgroundColor = grayColor,
                    FontSize = 8
                };

                addNewCourseButton.Clicked += EvaluateCustomOrStandardGame;
                coursesLayout.Children.Add(addNewCourseButton);
            }

            //Add the list of courses in the database
            for (int i = 0; i < courseList.Count(); i++)
            {
                Button courseNameButton = new Button()
                {
                    Text = courseList[i],
                    BackgroundColor = greenColor,
                    FontSize = 8
                };

                if (courseLookupPage)
                {
                    courseNameButton.Clicked += ListCourseDetails;
                }
                else
                {
                    courseNameButton.Clicked += EvaluateCustomOrStandardGame;
                }
                coursesLayout.Children.Add(courseNameButton);
            }
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

            Grid g = new Grid
            {
                RowDefinitions =
                {
                },
                ColumnDefinitions =
                {
                    new ColumnDefinition{ Width = 70 },
                    new ColumnDefinition{ Width = 70 },
                    new ColumnDefinition{ Width = 70 }
                }
            };

            g.Children.Add(new BoxView
            {
                Color = grayColor
            }, 0, 0);

            g.Children.Add(new Label
            {
                Text = "Hole",
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
                FontSize = 8
            }, 0, 0);

            g.Children.Add(new BoxView
            {
                Color = grayColor
            }, 1, 0);

            g.Children.Add(new Label
            {
                Text = "Par",
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
                FontSize = 8
            }, 1, 0);

            g.Children.Add(new BoxView
            {
                Color = grayColor
            }, 2, 0);

            g.Children.Add(new Label
            {
                Text = "Score",
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
                FontSize = 8
            }, 2, 0);

            for (int i = 0; i < nineOrEighteen; i++)
            {
                g.Children.Add(new BoxView
                {
                    Color = grayColor
                }, 0, i + 1);

                g.Children.Add(new Label
                {
                    Text = Convert.ToString(i + 1),
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.Center,
                    FontSize = 8
                }, 0, i + 1);

                g.Children.Add(new BoxView
                {
                    Color = darkGreenColor
                }, 1, i + 1);

                g.Children.Add(new Label
                {
                    Text = Convert.ToString(courses.GetHolePar(currentCourseName, i + 1)),
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.Center,
                    FontSize = 8
                }, 1, i + 1);

                g.Children.Add(new BoxView
                {
                    Color = greenColor
                }, 2, i + 1);

                g.Children.Add(new Label
                {
                    Text = Convert.ToString(scoreCard[i]),
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.Center,
                    FontSize = 8
                }, 2, i + 1);
            }

            g.Children.Add(new BoxView
            {
                Color = grayColor
            }, 0, nineOrEighteen + 1);

            g.Children.Add(new Label
            {
                Text = "Total",
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
                FontSize = 8
            }, 0, nineOrEighteen + 1);

            g.Children.Add(new BoxView
            {
                Color = grayColor
            }, 1, nineOrEighteen + 1);

            g.Children.Add(new Label
            {
                Text = Convert.ToString(courses.GetCoursePar(currentCourseName)),
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
                FontSize = 8
            }, 1, nineOrEighteen + 1);

            g.Children.Add(new BoxView
            {
                Color = grayColor
            }, 2, nineOrEighteen + 1);

            g.Children.Add(new Label
            {
                Text = Convert.ToString(currentCourseScore),
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
                FontSize = 8
            }, 2, nineOrEighteen + 1);

            g.Children.Add(new BoxView
            {
                Color = grayColor
            }, 0, nineOrEighteen + 2);

            g.Children.Add(new Label
            {
                Text = "Ovr",
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
                FontSize = 8
            }, 0, nineOrEighteen + 2);

            g.Children.Add(new BoxView
            {
                Color = grayColor
            }, 2, nineOrEighteen + 2);

            g.Children.Add(new Label
            {
                Text = Convert.ToString(currentCourseScoreRelativeToPar),
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
                FontSize = 8
            }, 2, nineOrEighteen + 2);

            finalLayout.Children.Add(g);
            finalLayout.Children.Add(new Label { });

            MainPage.Navigation.PushAsync(fp);
            MainPage.Navigation.RemovePage(ssp);
            midRound = false;

            //Reset all round counters
            var currentGameQueryResult = dbConnection.Query<CurrentGame>("select * from CurrentGame").FirstOrDefault();
            if (currentGameQueryResult != null)
            {
                dbConnection.RunInTransaction(() =>
                {
                    dbConnection.Delete(currentGameQueryResult);
                });
            }

        }

        protected async void ListCourseDetails(object sender, System.EventArgs e)
        {
            Button removeButton = new Button() {
                Text = "Remove course",
                FontSize = 8
            };

            courseNameText = (sender as Button).Text;

            Label courseName = new Label()
            {
                Text = courseNameText,
                FontSize = 8
            };

            Label scoreCardLabel = new Label()
            {
                Text = "Hole/Par",
                FontSize = 6
            };

            Grid g = new Grid
            {
                RowDefinitions =
                {
                    new RowDefinition{ },
                    new RowDefinition { Height = new GridLength(2, GridUnitType.Star) },
                    new RowDefinition{ },
                    new RowDefinition { Height = new GridLength(2, GridUnitType.Star) },

                }
            };

            for (int i = 0; i < courses.GetNineOrEighteen(courseNameText); i++)
            {
                g.Children.Add(new BoxView
                {
                    Color = darkGreenColor
                }, i % 9, (2 * (i / 9)));

                g.Children.Add(new Label
                {
                    Text = Convert.ToString(i + 1),
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.Center,
                    FontSize = 5
                }, i % 9, (2 * (i / 9)));

                g.Children.Add(new BoxView
                {
                    Color = grayColor
                }, i % 9, (2 * (i / 9)) + 1);

                g.Children.Add(new Label
                {
                    Text = Convert.ToString(courses.GetHolePar(courseNameText, i + 1)),
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.Center,
                    FontSize = 5
                }, i % 9, (2 * (i / 9)) + 1);
            }

            courseDetailLayout = new AbsoluteLayout
            {
                Children =
                {
                    removeButton,
                    courseName,
                    scoreCardLabel,
                    g
                }
            };

            AbsoluteLayout.SetLayoutBounds(courseName, new Rectangle(0.5, 0.2, 250, 120));
            AbsoluteLayout.SetLayoutFlags(courseName, AbsoluteLayoutFlags.PositionProportional);
            AbsoluteLayout.SetLayoutBounds(removeButton, new Rectangle(0.5, .9, 150, 60));
            AbsoluteLayout.SetLayoutFlags(removeButton, AbsoluteLayoutFlags.PositionProportional);
            if (courses.GetNineOrEighteen(courseNameText) == 9)
            {
                AbsoluteLayout.SetLayoutBounds(g, new Rectangle(0.5, 0.55, 350, 100));
                AbsoluteLayout.SetLayoutFlags(g, AbsoluteLayoutFlags.PositionProportional);
                AbsoluteLayout.SetLayoutBounds(scoreCardLabel, new Rectangle(0.1, 0.65, 140, 60));
                AbsoluteLayout.SetLayoutFlags(scoreCardLabel, AbsoluteLayoutFlags.PositionProportional);
            }
            else
            {
                AbsoluteLayout.SetLayoutBounds(g, new Rectangle(0.5, 0.5, 350, 100));
                AbsoluteLayout.SetLayoutFlags(g, AbsoluteLayoutFlags.PositionProportional);
                AbsoluteLayout.SetLayoutBounds(scoreCardLabel, new Rectangle(0.1, 0.8, 140, 60));
                AbsoluteLayout.SetLayoutFlags(scoreCardLabel, AbsoluteLayoutFlags.PositionProportional);
            }

            removeButton.Clicked += DisplayRemoveCourseScreen;

            cdp.Content = courseDetailLayout;
            await MainPage.Navigation.PushAsync(cdp);
        }
        

        protected async void DisplayRemoveCourseScreen(object sender, System.EventArgs e)
        {
            areYouSureLabel.Text = "Delete all info for " + courseNameText + "?";
            if ((midRound == true) && (courseNameText == currentCourseName))
            {
                areYouReallySureLabel.Text = "You will lose all data for your current round";
            }
            else
            {
                areYouReallySureLabel.Text = "";
            }
            await MainPage.Navigation.PushAsync(dp);
        }
        protected void RemoveCourse(string courseName)
        {
            courses.RemoveCourse(courseName);
            GenerateCourseList(true);
            if (midRound)
            {
                if (currentCourseName == courseName)
                {
                    midRound = false;
                    var currentGameQueryResult = dbConnection.Query<CurrentGame>("select * from CurrentGame").FirstOrDefault();
                    if (currentGameQueryResult != null)
                    {
                        dbConnection.RunInTransaction(() =>
                        {
                            dbConnection.Delete(currentGameQueryResult);
                        });
                    }
                }
            }

            var courseQueryResult = dbConnection.Query<Course>("select * from Course where Name = '" + courseName + "'").FirstOrDefault();
            if (courseQueryResult != null)
            {
                dbConnection.RunInTransaction(() =>
                {
                    dbConnection.Delete(courseQueryResult);
                });
            }

            MainPage.Navigation.RemovePage(cdp);
        }

        protected void OnAboutButtonClicked(object sender, System.EventArgs e)
        {
            Toast.DisplayText("Andrew Johnson         github.com/AndrewAJoh");
        }

        protected void OnWaterAstheticButtonClicked(object sender, System.EventArgs e)
        {
            Toast.DisplayText("Splash!");
        }

        protected void OnYesDeleteButtonClicked(object sender, System.EventArgs e)
        {
            RemoveCourse(courseNameText);
            MainPage.Navigation.RemovePage(dp);

            if (courses.GetCourseCount() == 0)
            {
                MainPage.Navigation.RemovePage(clp);
            }
        }

        protected void OnNoDeleteButtonClicked(object sender, System.EventArgs e)
        {
            MainPage.Navigation.RemovePage(dp);
        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            if (midRound)
            {
                CurrentGame gameRecord = new CurrentGame
                {
                    CourseName = currentCourseName,
                    CurrentHole = currentHole,
                    FurthestHole = furthestHole,
                    Strokes = strokes
                };

                if (currentHole == furthestHole)
                {
                    scoreCard[currentHole - 1] = strokes;
                }

                var subScoreCard = "";
                var courseScore = 0;

                for (int i = 0; i < scoreCard.Length; i++)
                {
                    subScoreCard += Convert.ToString(scoreCard[i]);
                    courseScore += scoreCard[i];
                }

                gameRecord.Scorecard = subScoreCard;
                gameRecord.Id = 1;
                gameRecord.CurrentCourseScore = courseScore;

                dbConnection.InsertOrReplace(gameRecord);
            }
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
