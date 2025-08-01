using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class FindPlayer : MonoBehaviour
{
    AIPath _aiPath;
    AIDestinationSetter _aiDestinationSetter;

    private void Awake()
    {
        _aiPath = GetComponent<AIPath>();
        if (_aiPath == null)
            Debug.LogError("No AIPath Component On " + gameObject.name);

        _aiDestinationSetter = GetComponent<AIDestinationSetter>();
        if (_aiDestinationSetter == null)
            Debug.LogError("No AIDestinationSetter Component On " + gameObject.name);
    }

    void Start()
    {
        _aiDestinationSetter.target = Player.Instance?.transform; // 设置目标为玩家
        if (_aiDestinationSetter.target == null)
        {
            Debug.LogWarning("Player实例未找到，无法设置目标");
            return;
        }
    }
}
