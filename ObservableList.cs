public class ObservableList<T> : List<T>
{
    public event EventHandler ListChanged;

    public new void Add(T item)
    {
        base.Add(item);
        OnListChanged();
    }

    public new void Remove(T item)
    {
        base.Remove(item);
        OnListChanged();
    }

    protected virtual void OnListChanged()
    {
        ListChanged?.Invoke(this, EventArgs.Empty);
    }
}