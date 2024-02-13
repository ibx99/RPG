namespace RPG.Saving
{
    public interface ISaveable
    {
        void RestoreState(object state);
        object CaptureState();
    }    
}
