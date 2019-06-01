using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using Schedule.Models;
using System.Linq;
using Newtonsoft.Json;

namespace Schedule.ViewModels
{
    class DayViewModel
    {
        public List<Couple> Couples { get; set; }
        public Dictionary<byte, TeacherCouple> TeacherCouples { get; set; }
        public List<TeacherCouple> TeacherCoupleList { get; set; }

        public DayViewModel(string dayOfWeek)
        {
            #region Определяется номер недели (numOfWeek)
            string numOfWeek = "";
            if (App.Current.Properties.TryGetValue("numOfWeek", out object num))
            {
                numOfWeek = (string)num;
            }
            else
            {
                numOfWeek = "1";
            }
            #endregion

            //проверяется студент или преподаватель
            if (App.Current.Properties.TryGetValue("isTeacher", out object isTeacher))
            {
                if ((bool)isTeacher)
                {
                    #region Загрузка пар для Преподавателя
                    if (App.Current.Properties.TryGetValue("schedule" + dayOfWeek + numOfWeek, out object schedule))
                    {
                        //проверка на наличие расписания в коллекции Properties
                        TeacherCoupleList = JsonConvert.DeserializeObject<List<TeacherCouple>>(Convert.ToString(schedule));
                        return;
                    }
                    TeacherCouples = new Dictionary<byte, TeacherCouple>();
                    //проверяется имя преподавателя
                    if (App.Current.Properties.TryGetValue("teacherName", out object AppTeacherName))
                    {
                        string thisTeacher = (string) AppTeacherName;
                        foreach (var f in App.facultiesJSON)
                        {
                            foreach (var g in f.Groups)
                            {
                                foreach (var c in g.Couples)
                                {
                                    //Contains, так как в паре английского может быть несколько преподавателей
                                    if (c.CoupleTeacher.Contains(thisTeacher)) 
                                    {
                                        if (c.Week == numOfWeek)
                                        {
                                            if (c.Day == dayOfWeek)
                                            {
                                                byte coupleNum = Convert.ToByte(c.CoupleNum);
                                                //проверка на наличие пары с таким номером
                                                if (!TeacherCouples.ContainsKey(coupleNum))
                                                {
                                                    //Если пары с таким номером еще нет, то пара просто добавляется в коллекцию
                                                    TeacherCouples.Add(coupleNum, new TeacherCouple(c, g.GroupId + (c.SubgroupId != null ? "(" + c.SubgroupId + ")" : "")));
                                                }
                                                else
                                                {
                                                    //Если пара с таким номером уже есть, то в поле CoupleTeacher этой пары дописывается номер новой группы
                                                    TeacherCouples[coupleNum].CoupleTeacher += ", " + g.GroupId + (c.SubgroupId != null ? "(" + c.SubgroupId + ")" : "");
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    //сортировка пар, так как могут находится не в правильном порядке
                    if (TeacherCouples != null)
                    {
                        SortedDictionary<byte, TeacherCouple> sortedTeacherCouples = new SortedDictionary<byte, TeacherCouple>(TeacherCouples);
                        TeacherCoupleList = sortedTeacherCouples.Values.ToList();
                    }
                    //Сохранение расписания, для дальнейшего использования
                    string json = JsonConvert.SerializeObject(TeacherCoupleList);
                    App.Current.Properties.Add("schedule" + dayOfWeek + numOfWeek, json);
                    #endregion
                }
                else 
                {
                    #region Загрузка пар для Студента
                    if (App.Current.Properties.TryGetValue("schedule" + dayOfWeek + numOfWeek, out object schedule))
                    {
                        //проверка на наличие собранного расписания в коллекции Properties
                        Couples = JsonConvert.DeserializeObject<List<Couple>>(Convert.ToString(schedule));
                        return;
                    }
                    Couples = new List<Couple>();
                    //проверяется факультет
                    if (App.Current.Properties.TryGetValue("facultyName", out object FacultyName))
                    {
                        string facultyName = (string)FacultyName;
                        //проверяются номер группы, имя группы и подгруппа
                        string groupId = "";
                        if (App.Current.Properties.TryGetValue("groupId", out object GroupId))
                        { groupId = (string)GroupId; }
                        string groupName = "";
                        if (App.Current.Properties.TryGetValue("groupName", out object GroupName))
                        { groupName = (string)GroupName; }
                        string subgroup = null;
                        if (App.Current.Properties.TryGetValue("subgroup", out object Subgroup))
                        { subgroup = (string)Subgroup; }

                        foreach (var f in App.facultiesJSON)
                        {
                            if (f.FacultyName == facultyName)
                            {
                                foreach (var g in f.Groups)
                                {
                                    if (g.GroupId == groupId && g.GroupName == groupName)
                                    {
                                        foreach (var c in g.Couples)
                                        {
                                            if (c.Week == numOfWeek && c.SubgroupName == subgroup && c.Day == dayOfWeek)
                                            {
                                                Couples.Add(c);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        //Сохранение расписания, для дальнейшего использования
                        string json = JsonConvert.SerializeObject(Couples);
                        App.Current.Properties.Add("schedule" + dayOfWeek + numOfWeek, json);
                    }
                    #endregion
                }
            }
        }
    }
}
