using UnityEngine;
using System;
using DG.Tweening;
using System.Collections;
using UnityEngine.Events;

public class CombinationLock : InspectObject
{
    [Header("Combination Lock Settings")]
    [SerializeField] private float _lockRotationDuration = 0.5f;
    public Combination[] locks = new Combination[3];
    public UnityEvent unlockEvent;

    private bool _canChange = true;

    public void IncrementNumber(int lockIndex)
    {
        if (_canChange) Calculate(lockIndex, 1);
    }

    public void DecrementNumber(int lockIndex)
    {
        if (_canChange) Calculate(lockIndex, -1);
    }

    public void Calculate(int lockIndex, int direction)
    {
        Vector3 rot = locks[lockIndex].cylinder.transform.localEulerAngles;

        if (direction > 0) rot.y += 36;
        else if (direction < 0) rot.y -= 36;

        locks[lockIndex].currentNumber += direction;

        locks[lockIndex].currentNumber = 
            (locks[lockIndex].currentNumber + 10) % 10;

        //locks[lockIndex].cylinder.transform.localEulerAngles = rot;
        locks[lockIndex].cylinder.transform.
            DOLocalRotateQuaternion(Quaternion.Euler(rot), 
            _lockRotationDuration);

        CheckCombination();
        StartCoroutine(NumberChangeCooldown());

        if (isInspecting)
        {
            original.GetComponent<CombinationLock>().
                Calculate(lockIndex, direction);
        }
    }
    private void CheckCombination()
    {
        // Check combination after every button action.
        int correctNumbers = 0;

        foreach (Combination item in locks)
        {

            if (item.currentNumber == item.requiredNumber)
            {
                correctNumbers += 1;
            }
        }
        if (correctNumbers == 3)
        {
            /* Destroy combination lock (gameobject) and door attached to 
             * this script. 
             */
            DOTween.Clear();
            DisableInspect();
            unlockEvent.Invoke();
        }
    }
    private IEnumerator NumberChangeCooldown()
    {
        _canChange = false;
        yield return new WaitForSeconds(_lockRotationDuration);
        _canChange = true;
    }
}

[Serializable]
public struct Combination
{
    public int currentNumber;
    public int requiredNumber;
    public Transform cylinder;
}