using System.Collections.Generic;
using UnityEngine;
using System.Linq;


public class LifeCounterObjects : MonoBehaviour
{
	[SerializeField] private List<LifeCounter> _objects;
	private bool isShowing;


    public void Show()
    {
		if(isShowing) return;
		isShowing = true;
		int n = 0;
        _objects.ForEach(obj=>obj.Show(n++));
    }

    public void Hide()
    {
		if(!isShowing) return;
		isShowing = false;
        _objects.ForEach(obj=>obj.Hide());
    }

	public void ActivateNext()
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
		isShowing = false;
		_objects.ForEach(obj=>obj.Reset());
	}
}