﻿using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ResourceCounterPresenter : OnMessage<MemberStateChanged>
{
    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI counter;

    private Member _member;
    private IResourceType _resourceType;
    
    public void Hide()
    {
        gameObject.SetActive(false);
    }
    
    public void Init(Member member, IResourceType resource)
    {
        _member = member;
        _resourceType = resource;
        icon.sprite = resource.Icon;
        counter.text = $"{resource.StartingAmount}/{resource.MaxAmount}";
    }

    protected override void Execute(MemberStateChanged msg)
    {
        if (msg.State.MemberId != _member.Id) return;
        
        counter.text = $"{msg.State[_resourceType]}/{_resourceType.MaxAmount}";
    }
}
