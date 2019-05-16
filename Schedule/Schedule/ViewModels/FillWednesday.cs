//using Schedule.Models;
//using System;
//using System.Collections.Generic;
//using System.ComponentModel;
//using System.Text;

//using Plugin.Settings;

//namespace Schedule.ViewModels
//{
//    class FillWednesday : INotifyPropertyChanged
//    {
//        public List<Couple> couples { get; set; }

//        public event PropertyChangedEventHandler PropertyChanged;

//        public FillWednesday()
//        {
//            couples = new List<Couple>();

//            object isTeacher = ""; //проверяется студент или преподаватель
//            if (App.Current.Properties.TryGetValue("isTeacher", out isTeacher))
//            {
//                if ((bool)isTeacher)
//                {
//                    // выполняем действия
//                }
//                else
//                {
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
//                                                if (group.Value.secondSubgroup.weeks.firstWeek.wednesday.couple1.coupleName != null)
//                                                    couples.Add(group.Value.secondSubgroup.weeks.firstWeek.wednesday.couple1);
//                                                if (group.Value.secondSubgroup.weeks.firstWeek.wednesday.couple2.coupleName != null)
//                                                    couples.Add(group.Value.secondSubgroup.weeks.firstWeek.wednesday.couple2);
//                                                if (group.Value.secondSubgroup.weeks.firstWeek.wednesday.couple3.coupleName != null)
//                                                    couples.Add(group.Value.secondSubgroup.weeks.firstWeek.wednesday.couple3);
//                                                if (group.Value.secondSubgroup.weeks.firstWeek.wednesday.couple4.coupleName != null)
//                                                    couples.Add(group.Value.secondSubgroup.weeks.firstWeek.wednesday.couple4);
//                                            }
//                                            else
//                                            {
//                                                if (group.Value.secondSubgroup.weeks.secondWeek.wednesday.couple1.coupleName != null)
//                                                    couples.Add(group.Value.secondSubgroup.weeks.secondWeek.wednesday.couple1);
//                                                if (group.Value.secondSubgroup.weeks.secondWeek.wednesday.couple2.coupleName != null)
//                                                    couples.Add(group.Value.secondSubgroup.weeks.secondWeek.wednesday.couple2);
//                                                if (group.Value.secondSubgroup.weeks.secondWeek.wednesday.couple3.coupleName != null)
//                                                    couples.Add(group.Value.secondSubgroup.weeks.secondWeek.wednesday.couple3);
//                                                if (group.Value.secondSubgroup.weeks.secondWeek.wednesday.couple4.coupleName != null)
//                                                    couples.Add(group.Value.secondSubgroup.weeks.secondWeek.wednesday.couple4);
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
//                                                if (group.Value.firstSubgroup.weeks.firstWeek.wednesday.couple1.coupleName != null)
//                                                    couples.Add(group.Value.firstSubgroup.weeks.firstWeek.wednesday.couple1);
//                                                if (group.Value.firstSubgroup.weeks.firstWeek.wednesday.couple2.coupleName != null)
//                                                    couples.Add(group.Value.firstSubgroup.weeks.firstWeek.wednesday.couple2);
//                                                if (group.Value.firstSubgroup.weeks.firstWeek.wednesday.couple3.coupleName != null)
//                                                    couples.Add(group.Value.firstSubgroup.weeks.firstWeek.wednesday.couple3);
//                                                if (group.Value.firstSubgroup.weeks.firstWeek.wednesday.couple4.coupleName != null)
//                                                    couples.Add(group.Value.firstSubgroup.weeks.firstWeek.wednesday.couple4);
//                                            }
//                                            else
//                                            {
//                                                if (group.Value.firstSubgroup.weeks.secondWeek.wednesday.couple1.coupleName != null)
//                                                    couples.Add(group.Value.firstSubgroup.weeks.secondWeek.wednesday.couple1);
//                                                if (group.Value.firstSubgroup.weeks.secondWeek.wednesday.couple2.coupleName != null)
//                                                    couples.Add(group.Value.firstSubgroup.weeks.secondWeek.wednesday.couple2);
//                                                if (group.Value.firstSubgroup.weeks.secondWeek.wednesday.couple3.coupleName != null)
//                                                    couples.Add(group.Value.firstSubgroup.weeks.secondWeek.wednesday.couple3);
//                                                if (group.Value.firstSubgroup.weeks.secondWeek.wednesday.couple4.coupleName != null)
//                                                    couples.Add(group.Value.firstSubgroup.weeks.secondWeek.wednesday.couple4);
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
