using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Linq;
public static class InputBindingHelpers
{
    // Start is called before the first frame update
   
    public static List<string> GetBindingPaths(string layoutName)
    {
        List<string> bindingPaths = new List<string>();
        RecurseLayout(layoutName, bindingPaths);
        return bindingPaths;
    }

    static bool RecurseLayout(string name, List<string> bindingPaths, Stack<string> pathStack = null)
    {
        var layout = InputSystem.LoadLayout(name);

        if (pathStack == null)
        {
            pathStack = new Stack<string>();
            if (layout.isGenericTypeOfDevice)
                pathStack.Push($"<{name}>");
            else
                pathStack.Push(name);
        }

        //has no children, is a binding path
        if (layout.controls.Count == 0)
            return false;

        foreach (var c in layout.controls)
        {
            pathStack.Push(c.name);

            if (!RecurseLayout(c.layout, bindingPaths, pathStack))
            {
                bindingPaths.Add(string.Join("/", pathStack.Reverse()));
                pathStack.Pop();
            }
        }

        pathStack.Pop();

        return true;
    }
}