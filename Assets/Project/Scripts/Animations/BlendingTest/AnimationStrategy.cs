using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Animancer;

public abstract class AnimationStrategy
{
    abstract public Animancer.AnimancerState IAnimationPlay();
}

public class NormalizedAnimationPlay : AnimationStrategy
{
    public override Animancer.AnimancerState IAnimationPlay()
    {
        throw new System.NotImplementedException();
    }

}

public class DefaultAnimationPlay : AnimationStrategy
{
    public override Animancer.AnimancerState IAnimationPlay()
    {
        throw new System.NotImplementedException();
    }
}
