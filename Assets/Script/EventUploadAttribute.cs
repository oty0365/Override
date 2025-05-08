using System;

[AttributeUsage(AttributeTargets.Method)]
public class EventUploadAttribute : Attribute
{
    public Action gameAction;
    public void CustomEventAttribute(Action action)
    {
        gameAction = action;
    }
}
