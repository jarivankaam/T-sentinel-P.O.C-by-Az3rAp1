public interface IRoutineRepository
{
    void InsertRoutine(Routine routine);
    public List<Routine> GetRoutines();
}