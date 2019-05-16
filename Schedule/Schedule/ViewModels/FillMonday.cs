//using Schedule.Models;
//using System;
//using System.Collections.Generic;
//using System.ComponentModel;
//using System.Text;

//using Plugin.Settings;
//using System.Threading;
//using System.Threading.Tasks;
//using System.Linq;

//namespace Schedule.ViewModels
//{
//    class FillMonday : INotifyPropertyChanged
//    {
//        public List<Couple> couples { get; set; }

//        public Dictionary<byte, TeacherCouple> teacherCouples { get; set; }
//        public List<TeacherCouple> teacherCoupleList { get; set; }

//        public event PropertyChangedEventHandler PropertyChanged;

//        public FillMonday()
//        {
            

//            object isTeacher = ""; //проверяется студент или преподаватель
//            if (App.Current.Properties.TryGetValue("isTeacher", out isTeacher))
//            {
//                /************************Преподаватель***************************************/
//                if ((bool)isTeacher)
//                {
//                    teacherCouples = new Dictionary<byte, TeacherCouple>();

//                    object AppTeacherName = ""; //проверяется номер группы
//                    if (App.Current.Properties.TryGetValue("teacherName", out AppTeacherName))
//                    {
//                        object AppNumOfWeek = ""; //проверяется номер недели
//                        if (App.Current.Properties.TryGetValue("numOfWeek", out AppNumOfWeek))
//                        {
//                            if (AppNumOfWeek.ToString() == "1")
//                            {
//                                foreach (KeyValuePair<string, Group> group in App.sched)
//                                {

//                                    #region 1 couple
//                                    if (group.Value.firstSubgroup.weeks.firstWeek.monday.couple1.coupleTeacher == (string)AppTeacherName)
//                                        if (!teacherCouples.ContainsKey(1))
//                                            if (group.Value.secondSubgroup == null)
//                                                teacherCouples.Add(1, new TeacherCouple(group.Value.firstSubgroup.weeks.firstWeek.monday.couple1, group.Value.groupId));
//                                            else
//                                                teacherCouples.Add(1, new TeacherCouple(group.Value.firstSubgroup.weeks.firstWeek.monday.couple1, group.Value.groupId + " (1)"));
//                                        else
//                                            if (group.Value.secondSubgroup == null)
//                                            teacherCouples[1].coupleTeacher += ", " + group.Value.groupId;
//                                        else
//                                            teacherCouples[1].coupleTeacher += ", " + group.Value.groupId + " (1)";
//                                    #endregion
//                                    #region 2 couple
//                                    if (group.Value.firstSubgroup.weeks.firstWeek.monday.couple2.coupleTeacher == (string)AppTeacherName)
//                                        if (!teacherCouples.ContainsKey(2))
//                                            if (group.Value.secondSubgroup == null)
//                                                teacherCouples.Add(2, new TeacherCouple(group.Value.firstSubgroup.weeks.firstWeek.monday.couple2, group.Value.groupId));
//                                            else
//                                                teacherCouples.Add(2, new TeacherCouple(group.Value.firstSubgroup.weeks.firstWeek.monday.couple2, group.Value.groupId + " (1)"));
//                                        else
//                                            if (group.Value.secondSubgroup == null)
//                                            teacherCouples[2].coupleTeacher += ", " + group.Value.groupId;
//                                        else
//                                            teacherCouples[2].coupleTeacher += ", " + group.Value.groupId + " (1)";
//                                    #endregion
//                                    #region 3 couple
//                                    if (group.Value.firstSubgroup.weeks.firstWeek.monday.couple3.coupleTeacher == (string)AppTeacherName)
//                                        if (!teacherCouples.ContainsKey(3))
//                                            if (group.Value.secondSubgroup == null)
//                                                teacherCouples.Add(3, new TeacherCouple(group.Value.firstSubgroup.weeks.firstWeek.monday.couple3, group.Value.groupId));
//                                            else
//                                                teacherCouples.Add(3, new TeacherCouple(group.Value.firstSubgroup.weeks.firstWeek.monday.couple3, group.Value.groupId + " (1)"));
//                                        else
//                                            if (group.Value.secondSubgroup == null)
//                                            teacherCouples[3].coupleTeacher += ", " + group.Value.groupId;
//                                        else
//                                            teacherCouples[3].coupleTeacher += ", " + group.Value.groupId + " (1)";
//                                    #endregion
//                                    #region 4 couple
//                                    if (group.Value.firstSubgroup.weeks.firstWeek.monday.couple4.coupleTeacher == (string)AppTeacherName)
//                                        if (!teacherCouples.ContainsKey(4))
//                                            if (group.Value.secondSubgroup == null)
//                                                teacherCouples.Add(4, new TeacherCouple(group.Value.firstSubgroup.weeks.firstWeek.monday.couple4, group.Value.groupId));
//                                            else
//                                                teacherCouples.Add(4, new TeacherCouple(group.Value.firstSubgroup.weeks.firstWeek.monday.couple4, group.Value.groupId + " (1)"));
//                                        else
//                                            if (group.Value.secondSubgroup == null)
//                                            teacherCouples[4].coupleTeacher += ", " + group.Value.groupId;
//                                        else
//                                            teacherCouples[4].coupleTeacher += ", " + group.Value.groupId + " (1)";
//                                    #endregion

//                                    if (group.Value.secondSubgroup != null)
//                                    {
//                                        #region 1 couple
//                                        if (group.Value.secondSubgroup.weeks.firstWeek.monday.couple1.coupleTeacher == (string)AppTeacherName)
//                                            if (!teacherCouples.ContainsKey(1))
//                                                teacherCouples.Add(1, new TeacherCouple(group.Value.secondSubgroup.weeks.firstWeek.monday.couple1, group.Value.groupId + " (2)"));
//                                            else
//                                                teacherCouples[1].coupleTeacher += ", " + group.Value.groupId + " (2)";
//                                        #endregion
//                                        #region 2 couple
//                                        if (group.Value.secondSubgroup.weeks.firstWeek.monday.couple2.coupleTeacher == (string)AppTeacherName)
//                                            if (!teacherCouples.ContainsKey(2))
//                                                teacherCouples.Add(2, new TeacherCouple(group.Value.secondSubgroup.weeks.firstWeek.monday.couple2, group.Value.groupId + " (2)"));
//                                            else
//                                                teacherCouples[2].coupleTeacher += ", " + group.Value.groupId + " (2)";
//                                        #endregion
//                                        #region 3 couple
//                                        if (group.Value.secondSubgroup.weeks.firstWeek.monday.couple3.coupleTeacher == (string)AppTeacherName)
//                                            if (!teacherCouples.ContainsKey(3))
//                                                teacherCouples.Add(3, new TeacherCouple(group.Value.secondSubgroup.weeks.firstWeek.monday.couple3, group.Value.groupId + " (2)"));
//                                            else
//                                                teacherCouples[3].coupleTeacher += ", " + group.Value.groupId + " (2)";
//                                        #endregion
//                                        #region 4 couple
//                                        if (group.Value.secondSubgroup.weeks.firstWeek.monday.couple4.coupleTeacher == (string)AppTeacherName)
//                                            if (!teacherCouples.ContainsKey(4))
//                                                teacherCouples.Add(4, new TeacherCouple(group.Value.secondSubgroup.weeks.firstWeek.monday.couple4, group.Value.groupId + " (2)"));
//                                            else
//                                                teacherCouples[4].coupleTeacher += ", " + group.Value.groupId + " (2)";
//                                        #endregion
//                                    }
//                                }
//                            }
//                            else
//                            {
//                                foreach (KeyValuePair<string, Group> group in App.sched)
//                                {
//                                    #region 1 couple
//                                    if (group.Value.firstSubgroup.weeks.secondWeek.monday.couple1.coupleTeacher == (string)AppTeacherName)
//                                        if (!teacherCouples.ContainsKey(1))
//                                            if (group.Value.secondSubgroup == null)
//                                                teacherCouples.Add(1, new TeacherCouple(group.Value.firstSubgroup.weeks.secondWeek.monday.couple1, group.Value.groupId));
//                                            else
//                                                teacherCouples.Add(1, new TeacherCouple(group.Value.firstSubgroup.weeks.secondWeek.monday.couple1, group.Value.groupId + " (1)"));
//                                        else
//                                            if (group.Value.secondSubgroup == null)
//                                            teacherCouples[1].coupleTeacher += ", " + group.Value.groupId;
//                                        else
//                                            teacherCouples[1].coupleTeacher += ", " + group.Value.groupId + " (1)";
//                                    #endregion
//                                    #region 2 couple
//                                    if (group.Value.firstSubgroup.weeks.secondWeek.monday.couple2.coupleTeacher == (string)AppTeacherName)
//                                        if (!teacherCouples.ContainsKey(2))
//                                            if (group.Value.secondSubgroup == null)
//                                                teacherCouples.Add(2, new TeacherCouple(group.Value.firstSubgroup.weeks.secondWeek.monday.couple2, group.Value.groupId));
//                                            else
//                                                teacherCouples.Add(2, new TeacherCouple(group.Value.firstSubgroup.weeks.secondWeek.monday.couple2, group.Value.groupId + " (1)"));
//                                        else
//                                            if (group.Value.secondSubgroup == null)
//                                            teacherCouples[2].coupleTeacher += ", " + group.Value.groupId;
//                                        else
//                                            teacherCouples[2].coupleTeacher += ", " + group.Value.groupId + " (1)";
//                                    #endregion
//                                    #region 3 couple
//                                    if (group.Value.firstSubgroup.weeks.secondWeek.monday.couple3.coupleTeacher == (string)AppTeacherName)
//                                        if (!teacherCouples.ContainsKey(3))
//                                            if (group.Value.secondSubgroup == null)
//                                                teacherCouples.Add(3, new TeacherCouple(group.Value.firstSubgroup.weeks.secondWeek.monday.couple3, group.Value.groupId));
//                                            else
//                                                teacherCouples.Add(3, new TeacherCouple(group.Value.firstSubgroup.weeks.secondWeek.monday.couple3, group.Value.groupId + " (1)"));
//                                        else
//                                            if (group.Value.secondSubgroup == null)
//                                            teacherCouples[3].coupleTeacher += ", " + group.Value.groupId;
//                                        else
//                                            teacherCouples[3].coupleTeacher += ", " + group.Value.groupId + " (1)";
//                                    #endregion
//                                    #region 4 couple
//                                    if (group.Value.firstSubgroup.weeks.secondWeek.monday.couple4.coupleTeacher == (string)AppTeacherName)
//                                        if (!teacherCouples.ContainsKey(4))
//                                            if (group.Value.secondSubgroup == null)
//                                                teacherCouples.Add(4, new TeacherCouple(group.Value.firstSubgroup.weeks.secondWeek.monday.couple4, group.Value.groupId));
//                                            else
//                                                teacherCouples.Add(4, new TeacherCouple(group.Value.firstSubgroup.weeks.secondWeek.monday.couple4, group.Value.groupId + " (1)"));
//                                        else
//                                            if (group.Value.secondSubgroup == null)
//                                            teacherCouples[4].coupleTeacher += ", " + group.Value.groupId;
//                                        else
//                                            teacherCouples[4].coupleTeacher += ", " + group.Value.groupId + " (1)";
//                                    #endregion

//                                    if (group.Value.secondSubgroup != null)
//                                    {
//                                        #region 1 couple
//                                        if (group.Value.secondSubgroup.weeks.secondWeek.monday.couple1.coupleTeacher == (string)AppTeacherName)
//                                            if (!teacherCouples.ContainsKey(1))
//                                                teacherCouples.Add(1, new TeacherCouple(group.Value.secondSubgroup.weeks.secondWeek.monday.couple1, group.Value.groupId + " (2)"));
//                                            else
//                                                teacherCouples[1].coupleTeacher += ", " + group.Value.groupId + " (2)";
//                                        #endregion
//                                        #region 2 couple
//                                        if (group.Value.secondSubgroup.weeks.secondWeek.monday.couple2.coupleTeacher == (string)AppTeacherName)
//                                            if (!teacherCouples.ContainsKey(2))
//                                                teacherCouples.Add(2, new TeacherCouple(group.Value.secondSubgroup.weeks.secondWeek.monday.couple2, group.Value.groupId + " (2)"));
//                                            else
//                                                teacherCouples[2].coupleTeacher += ", " + group.Value.groupId + " (2)";
//                                        #endregion
//                                        #region 3 couple
//                                        if (group.Value.secondSubgroup.weeks.secondWeek.monday.couple3.coupleTeacher == (string)AppTeacherName)
//                                            if (!teacherCouples.ContainsKey(3))
//                                                teacherCouples.Add(3, new TeacherCouple(group.Value.secondSubgroup.weeks.secondWeek.monday.couple3, group.Value.groupId + " (2)"));
//                                            else
//                                                teacherCouples[3].coupleTeacher += ", " + group.Value.groupId + " (2)";
//                                        #endregion
//                                        #region 4 couple
//                                        if (group.Value.secondSubgroup.weeks.secondWeek.monday.couple4.coupleTeacher == (string)AppTeacherName)
//                                            if (!teacherCouples.ContainsKey(4))
//                                                teacherCouples.Add(4, new TeacherCouple(group.Value.secondSubgroup.weeks.secondWeek.monday.couple4, group.Value.groupId + " (2)"));
//                                            else
//                                                teacherCouples[4].coupleTeacher += ", " + group.Value.groupId + " (2)";
//                                        #endregion
//                                    }
//                                }
//                            }
//                        }
//                    }

//                    //сортировка пар, так как могут находится не в правильном порядке
//                    SortedDictionary<byte, TeacherCouple> sortedTeacherCouples = new SortedDictionary<byte, TeacherCouple>(teacherCouples);

//                    teacherCoupleList = sortedTeacherCouples.Values.ToList();

//                }
//                /************************Студент*********************************************/
//                else
//                {
//                    couples = new List<Couple>();

//                    object AppGroupId = ""; //проверяется номер группы
//                    if (App.Current.Properties.TryGetValue("groupId", out AppGroupId))
//                    {
//                        foreach (KeyValuePair<string, Group> group in App.sched)
//                        {
//                            if (group.Value.groupId == AppGroupId.ToString())
//                            {
//                                object AppSubgroup = ""; //проверяется подгруппа
//                                if (App.Current.Properties.TryGetValue("subgroup", out AppSubgroup))
//                                {
//                                    if (AppSubgroup.ToString() == "secondSubgroup")
//                                    {
//                                        object AppNumOfWeek = ""; //проверяется номер недели
//                                        if (App.Current.Properties.TryGetValue("numOfWeek", out AppNumOfWeek))
//                                        {
//                                            if (AppNumOfWeek.ToString() == "1")
//                                            {
//                                                if (group.Value.secondSubgroup.weeks.firstWeek.monday.couple1.coupleName != null)
//                                                    couples.Add(group.Value.secondSubgroup.weeks.firstWeek.monday.couple1);
//                                                if (group.Value.secondSubgroup.weeks.firstWeek.monday.couple2.coupleName != null)
//                                                    couples.Add(group.Value.secondSubgroup.weeks.firstWeek.monday.couple2);
//                                                if (group.Value.secondSubgroup.weeks.firstWeek.monday.couple3.coupleName != null)
//                                                    couples.Add(group.Value.secondSubgroup.weeks.firstWeek.monday.couple3);
//                                                if (group.Value.secondSubgroup.weeks.firstWeek.monday.couple4.coupleName != null)
//                                                    couples.Add(group.Value.secondSubgroup.weeks.firstWeek.monday.couple4);
//                                            }
//                                            else
//                                            {
//                                                if (group.Value.secondSubgroup.weeks.secondWeek.monday.couple1.coupleName != null)
//                                                    couples.Add(group.Value.secondSubgroup.weeks.secondWeek.monday.couple1);
//                                                if (group.Value.secondSubgroup.weeks.secondWeek.monday.couple2.coupleName != null)
//                                                    couples.Add(group.Value.secondSubgroup.weeks.secondWeek.monday.couple2);
//                                                if (group.Value.secondSubgroup.weeks.secondWeek.monday.couple3.coupleName != null)
//                                                    couples.Add(group.Value.secondSubgroup.weeks.secondWeek.monday.couple3);
//                                                if (group.Value.secondSubgroup.weeks.secondWeek.monday.couple4.coupleName != null)
//                                                    couples.Add(group.Value.secondSubgroup.weeks.secondWeek.monday.couple4);
//                                            }
//                                        }

//                                    }
//                                    else //значит первая группа
//                                    {
//                                        object AppNumOfWeek = ""; //проверяется номер недели
//                                        if (App.Current.Properties.TryGetValue("numOfWeek", out AppNumOfWeek))
//                                        {
//                                            if (AppNumOfWeek.ToString() == "1")
//                                            {
//                                                if (group.Value.firstSubgroup.weeks.firstWeek.monday.couple1.coupleName != null)
//                                                    couples.Add(group.Value.firstSubgroup.weeks.firstWeek.monday.couple1);
//                                                if (group.Value.firstSubgroup.weeks.firstWeek.monday.couple2.coupleName != null)
//                                                    couples.Add(group.Value.firstSubgroup.weeks.firstWeek.monday.couple2);
//                                                if (group.Value.firstSubgroup.weeks.firstWeek.monday.couple3.coupleName != null)
//                                                    couples.Add(group.Value.firstSubgroup.weeks.firstWeek.monday.couple3);
//                                                if (group.Value.firstSubgroup.weeks.firstWeek.monday.couple4.coupleName != null)
//                                                    couples.Add(group.Value.firstSubgroup.weeks.firstWeek.monday.couple4);
//                                            }
//                                            else
//                                            {
//                                                if (group.Value.firstSubgroup.weeks.secondWeek.monday.couple1.coupleName != null)
//                                                    couples.Add(group.Value.firstSubgroup.weeks.secondWeek.monday.couple1);
//                                                if (group.Value.firstSubgroup.weeks.secondWeek.monday.couple2.coupleName != null)
//                                                    couples.Add(group.Value.firstSubgroup.weeks.secondWeek.monday.couple2);
//                                                if (group.Value.firstSubgroup.weeks.secondWeek.monday.couple3.coupleName != null)
//                                                    couples.Add(group.Value.firstSubgroup.weeks.secondWeek.monday.couple3);
//                                                if (group.Value.firstSubgroup.weeks.secondWeek.monday.couple4.coupleName != null)
//                                                    couples.Add(group.Value.firstSubgroup.weeks.secondWeek.monday.couple4);
//                                            }
//                                        }
//                                    }
//                                }
//                            }

//                        }
//                    }
//                }
//            }
//        }
//    }
//}
