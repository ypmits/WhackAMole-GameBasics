using System.Collections.Generic;
using UnityEngine;
using System.Linq;


public class LifeCounterObjects : MonoBehaviour
{
#pragma warning disable 649
	[SerializeField] private List<LifeCounter> _objects;
#pragma warning restore 649
	private bool IsShowing;


    public void Show()
    {
		if(IsShowing) return;
		IsShowing = true;
		
		int n = 0;
        _objects.ForEach(obj=>obj.Show(n++));
    }

    public void Hide()
    {
		if(!IsShowing) return;
		IsShowing = false;

        _objects.ForEach(obj=>obj.Hide());
    }

	public void TakeLife()
	{
		bool n = false;
		_objects.ForEach(obj=>{
			if(!obj.isActive && !n) {
				obj.Activate();
				n = true;
			}
		});
		bool allActivate = _objects.All(obj=>obj.isActive);
		if(allActivate) {
			Debug.Log("All are activated!");
			StateManager.gameEvent.Invoke(GameEvent.GameOver);
		}
	}

	public void Reset()
	{
		IsShowing = false;
		_objects.ForEach(obj=>obj.Reset());
	}
}