# Hymns App
## Overview
This project is meant to making taking attendence for hymns easier. It currently provides the option to take attendance for students, add new students, and update current students' information.

## Project Breakdown
### *HymnsApp*
- This is where 99% of the work is!!!!
- See details for breakdown of each file

### *HymnsApp.android*
- This is where the android specific code lives

### *HymnsApp.ios*
- This is where the ios specific code lives

## Details
### *HymnsApp*
_Note that each xaml file has an associated cs file (cs stands for c# in case you were wondering)._
_The xaml files are where the design and layout should take place and each object that is layed out in the xaml file is accessable in the cs file by its name._
_The associated cs file is where the link between the frontend and the backend happens. This is where any information/data structures are populated._

- `App.xaml`
	- This is the entry point of our app.
	- `App.xaml.cs` contains the setup for our app and calls our first page (`MainPage.xaml`).
- `MainPage.xaml`
	- This is our first page where the user selects a grade.
- `GradeTabbedPage.xaml`
	- This is where you select what pages are going to make up your tabs.
- `GradeAttendance.xaml`
	- This is the default tab.
	- This page is where the user selects which students are present.
- `StudentInfo.xaml`
	- This the tab for student information.
	- This is where we display the students for the selected grade and are able to edit or add new students
- `EditAddStudent.xaml`
	- This page is where students can be editted or added.
	- This gets invoked when we search for a new student while taking attendance (`GradeAttenance.xaml`) or when we want to edit or add a student in the info page (`StudentInfo.xaml`).
- `AssemblyInfo.cs`
	- I don't think we need to worry about this

- `HymnsAttendenceProvider.cs`
	- This is where the backend lives.
	- This provides all the necessary methods that are used by the frontend.

### *HymnsApp.android*
- Images pertaining to the app are stored under	`Resources/drawable`
- _please update this if you find any other uses_

### *HymnsApp.ios*
- This is where the ios specific code lives
- I have no idea what to do here yet but it's probably important

# Additions
- There are some small bugs that need to be fixed (i.e attendence for future dates)
- Design and create a database that will replace the current file for persisting information.
- Update `HymnsAttendanceProvider.cs` to read and write to a database instead of a file.
- Get the ios version working and design it. Most of the code is shared with the android version but we need to get it running.
- Discuss and implement other information to store and/or display about students
- Add a tab for curriculum
- Add placeholder/ability to add student pictures
- Add a loading screen (splash screen)
- Future: facial recognition?