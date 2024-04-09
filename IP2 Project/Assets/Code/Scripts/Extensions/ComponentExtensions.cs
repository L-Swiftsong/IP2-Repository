using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ComponentExtensions
{
    public static bool TryGetComponentThroughParents<T>(this Component thisComponent, out T outputComponent) where T : class
    {
        // Assign the output component so that we can return false if we don't find it.
        outputComponent = null;

        // Search from this transform up the scene tree.
        Transform targetTransform = thisComponent.transform;
        while(targetTransform != null)
        {
            // If this transform contains the desired component, assign it to the outputComponent and return true.
            if (targetTransform.TryGetComponent<T>(out outputComponent))
                return true;

            // Search up the scene tree via the parents.
            targetTransform = targetTransform.parent;
        }

        // We didn't find the component, so return false.
        return false;
    }
    public static bool TryGetComponentThroughParents<T>(this Component thisComponent, out T outputComponent, int maxSearches) where T : class
    {
        // Assign the output component so that we can return false if we don't find it.
        outputComponent = null;

        // Search from this transform up the scene tree.
        Transform targetTransform = thisComponent.transform;
        int searchesRemaining = maxSearches;
        while (targetTransform != null || searchesRemaining == 0)
        {
            // If this transform contains the desired component, assign it to the outputComponent and return true.
            if (targetTransform.TryGetComponent<T>(out outputComponent))
                return true;

            // Search up the scene tree via the parents.
            targetTransform = targetTransform.parent;
            searchesRemaining--;
        }

        // We didn't find the component, so return false.
        return false;
    }


    public static bool TryApplyForce(this Component thisComponent, Vector2 force, float duration = 0.1f, ForceMode2D forceMode = ForceMode2D.Impulse)
    {
        // Search for an IMoveable.
        if (thisComponent.TryGetComponentThroughParents(out IMoveable movementScript))
        {
            movementScript.ApplyKnockback(force, duration, forceMode);
            return true;
        }
        // Else, Search for a Rigidbody2D.
        else if (thisComponent.TryGetComponentThroughParents(out Rigidbody2D rb2D))
        {
            rb2D.AddForce(force, forceMode);
            return true;
        }
        
        // We couldn't find a method to apply a force, return false.
        return false;
    }
}
