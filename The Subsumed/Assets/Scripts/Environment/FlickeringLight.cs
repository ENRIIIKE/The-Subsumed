using System.Collections;
using UnityEngine;

public class FlickeringLight : MonoBehaviour
{
    [SerializeField] private float _minTime;
    [SerializeField] private float _maxTime;

    

    [SerializeField] private int minNumberOfFlicks;
    [SerializeField] private int maxNumberOfFlicks;

    private Light _light;

    void Start()
    {
        _light = GetComponent<Light>();
        StartCoroutine(FlickLight());
    }

    public IEnumerator FlickLight()
    {
        float waitSeconds = Random.Range(_minTime, _maxTime);
        yield return new WaitForSeconds(waitSeconds);

        int numberOfFlicks = Random.Range(minNumberOfFlicks, maxNumberOfFlicks);
        for (int i = 0; i <= numberOfFlicks; i++)
        {
            _light.enabled = !_light.enabled;
            float waitFlick = Random.Range(0.05f, 0.18f);
            yield return new WaitForSeconds(0.05f);
        }
        
        _light.enabled = true;
        StartCoroutine(FlickLight());
    }
}
