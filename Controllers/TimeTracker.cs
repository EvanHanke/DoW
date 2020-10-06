using UnityEngine;
using System.Collections.Generic;

public class TimeTracker : MonoBehaviour {
	public static TimeTracker me;

	public List<DayCounter> counters;
	public int day = 0;
	public string[] Days = {"Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday", "Sunday"};
	public string today {
		get{
			return Days[day%7];
		}
	}


	void Awake(){
		me = this;
		counters = new List<DayCounter>();
	}

	public static void AddCounter(string name, int days){
		me.counters.Add ( new DayCounter(name, days) );
	}

	public static int GetCounter(string name){
		foreach(DayCounter dc in me.counters){
			if(dc.key == name) return dc.days;
		}
		return 999;
	}

	public static void AdvanceTime(){
		me.day++;
		me.BroadcastMessage("AdvanceTime");
		me.BroadcastMessage("UpdateEnvironment");
		foreach(DayCounter dc in me.counters){
			dc.days--;
			if(dc.days <0 ) dc.days = 0;
		}
		EZStatInfo.UpdateStats();
	}
	public static void RecallTime(){
		me.day--;
		me.BroadcastMessage("RecallTime");
		me.BroadcastMessage("UpdateEnvironment");
	}

	public static TimeSave TimeSave(){
		return new TimeSave(me.day, me.counters);
	}
	public static void LoadSave(TimeSave s){
		me.day = s.day;
		me.counters.AddRange(s.counters);
	}
}

[System.Serializable]
public class TimeSave{
	public int day;
	public DayCounter[] counters;

	public TimeSave(int d, List<DayCounter> l){
		day = d;
		counters = l.ToArray();
	}
}

[System.Serializable]
public class DayCounter{

	public DayCounter(string n, int d){
		key = n;
		days = d;
	}

	public string key;
	public int days;
}