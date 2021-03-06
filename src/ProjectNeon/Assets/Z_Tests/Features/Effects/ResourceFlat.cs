﻿using NUnit.Framework;

public sealed class ResourceFlat
{
    private EffectData data = new EffectData { 
        EffectType = EffectType.ResourceFlat, 
        FloatAmount = new FloatReference(5), 
        EffectScope = new StringReference(Ammo.Name) 
    } ;

    private static IResourceType Ammo = new InMemoryResourceType {Name = "Ammo", MaxAmount = 99, StartingAmount = 0};
    
    [Test]
    public void ResourceFlat_ApplyEffect()
    {
        var performer = TestMembers.Create(s => s, Ammo);
        
        AllEffects.Apply(data, performer, new Single(performer));
        
        Assert.AreEqual(5, performer.State[Ammo]);
    }
}
