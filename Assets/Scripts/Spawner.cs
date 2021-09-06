using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Schema;
using UnityEngine;
using UnityEngine.Timeline;

public class Spawner : MonoBehaviour {
    [SerializeField] private GameObject _spawnedPrefab = null;
    private GameObject _spawnedObj = null;
    
    // Grow the animation out of the card
    [SerializeField] private float _minScale = 0.1f;
    private Vector3 _targetScale = Vector3.one;
    [SerializeField] private float _growRate = 0.1f;
    //[SerializeField] private float _
    
    // extra effects of the the spawning animation
    [SerializeField] private GameObject _spawnParticlesPrefab = null;
    [SerializeField] private float _spawnParticleDuration = 1f;
    [SerializeField] private AudioClip _spawnSound = null;
    
    private bool _isDone = false;
    private bool _hasSpawned = false;

    public void SpawnPrefab() {
        if (_hasSpawned)
            return;
        _hasSpawned = true;

        if (_spawnedPrefab) {
            _spawnedObj = 
                GameObject.Instantiate(_spawnedPrefab, transform.parent);
                //GameObject.Instantiate(_spawnedPrefab, Vector3.zero, Quaternion.identity, transform.parent);
            //_spawnedObj.transform.position = Vector3.zero + (transform.parent.up * .001f);
            _spawnedObj.transform.position = Vector3.zero;
            // have the spawned object start at a small scale before 'growing' to the assigned scale
            _targetScale = _spawnedObj.transform.localScale;
            _spawnedObj.transform.localScale = new Vector3(_minScale, _minScale, _minScale);
            
            // 'grow' the spawned object
            StartCoroutine(COGrowSpawn());
        }
        else {
            _isDone = true;
        }

        // play the spawn sound effect
        if (_spawnSound)
            AudioManager.Instance.PlayAudioClip(_spawnSound);
        
        // instantiate and play particle effect
        if (_spawnParticlesPrefab) {
            var particleObj = GameObject.Instantiate(_spawnParticlesPrefab, transform.position, transform.rotation, transform.parent);
            var particles = particleObj.GetComponentsInChildren<ParticleSystem>();
            foreach (var x in particles) {
                x.Stop();
                var main = x.main;
                main.duration = _spawnParticleDuration;
                main.loop = false;
                x.Play();
            }
            GameObject.Destroy(particleObj, _spawnParticleDuration * 1.5f);
        }
        
        // TODO: create a button class that gets instantiated here and linked up to spawned object's controls
    }

    // coroutine to grow spawned obj
    IEnumerator COGrowSpawn() {
        Vector3 newScale = _spawnedObj.transform.localScale;
        Vector3 oldScale = newScale;
        float lerpVal = 0f;
        if (_growRate <= 0f)
            _growRate = 0.1f;
        while (true) {
            lerpVal = Mathf.Clamp(lerpVal + (_growRate * Time.deltaTime), 0f, 1f);
            newScale = Vector3.Lerp(oldScale, _targetScale, lerpVal);
            _spawnedObj.transform.localScale = newScale;
            if (lerpVal >= 1f)
                break;
            yield return null;
        }
        
        // mark spawner as done to delete self
        _isDone = true;
        
        // remove from image target to freely move (?)
        //_spawnedObj.transform.parent = null;
        //_spawnedObj.transform.parent = CombatManager.Instance.gameObject.transform;
        _spawnedObj.AddComponent<SimpleKeyboardController>();
        _spawnedObj.transform.position = transform.position;
    }

    // Update is called once per frame
    void Update() {
        if (_isDone) {
            GameObject.Destroy(this.gameObject);
        }
    }

    private void Start() {
        var observer = gameObject.GetComponentInParent<DefaultObserverEventHandler>();
        if (observer)
            observer.OnTargetFound.AddListener(this.SpawnPrefab);
    }

    private void OnDestroy() {
        // remove spawn function from the AR imagetarget observer
        var observer = gameObject.GetComponentInParent<DefaultObserverEventHandler>();
        if (observer)
            observer.OnTargetFound.RemoveListener(this.SpawnPrefab);
    }
}