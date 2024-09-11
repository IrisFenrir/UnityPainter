using System;

public class Property<T>
{
    private T m_value;
    private bool m_isLocked;

    public T Value
    {
        get { return m_value; }
        set
        {
            if (m_isLocked || m_value.Equals(value)) return;
            m_value = value;
            m_isLocked = true;
            OnValueChanged?.Invoke(value);
            m_isLocked = false;
        }
    }

    public Property(T origin = default)
    {
        m_value = origin;
    }

    public Action<T> OnValueChanged { get; set; }
}

