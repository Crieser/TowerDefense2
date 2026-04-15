using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;
using TowerDefense2.Views;

namespace TowerDefense2.ViewModels
{
    class MainViewModel
    {
        private UserControl currentViewLeft;
        private UserControl currentViewRight;

        public MainViewModel() 
        {
            // Init views and viewmodels
            StudentView studentView = new StudentView();
            StudentViewModel studentViewModel = new StudentViewModel();
            studentView.DataContext = studentViewModel;

            AddStudentView addStudentView = new AddStudentView();
            AddStudentViewModel addStudentViewModel = new AddStudentViewModel();
            addStudentView.DataContext = addStudentViewModel;

            //Set Uc
            CurrentViewLeft = studentView;
            CurrentViewRight = addStudentView;
        }

        public UserControl CurrentViewLeft
        {
            get { return currentViewLeft; }
            set { currentViewLeft = value; }
        }

        public UserControl CurrentViewRight
        {
            get { return currentViewRight; }
            set { currentViewRight = value; }
        }
    }
}
