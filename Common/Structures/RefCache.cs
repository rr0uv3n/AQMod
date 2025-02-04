﻿namespace Aequus.Common.Structures;

public record class RefCache<T>(RefFunc<T> GetReference) {
    private T _originalValue;
    private bool _hasOverridenValue;

    /// <summary>Replaces the referenced value with <paramref name="value"/>. The original value will be cached, and can be restored using <see cref="VoidOverriddenValue(ref T)"/>.</summary>
    /// <param name="value"></param>
    public void OverrideWith(T value) {
        if (!_hasOverridenValue) {
            _originalValue = GetReference();
            _hasOverridenValue = true;
        }
        //else {
        //    Log.Info(Environment.StackTrace);
        //}

        GetReference() = value;
    }

    /// <summary>Restores overriden values back to their original value.</summary>
    public void RestoreOriginalValue() {
        if (_hasOverridenValue) {
            GetReference() = _originalValue;
            _hasOverridenValue = false;
        }
    }
}
