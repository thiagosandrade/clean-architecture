namespace Domain.Activities;

public enum TaskActivityType
{
    TaskCreated = 0,

    TaskUpdated = 1,

    DescriptionUpdated = 2,

    PriorityChanged = 3,

    LabelsChanged = 4,

    DueDateChanged = 5,

    TaskCompleted = 6,

    TaskReopened = 7,

    SubtasksUpdated = 8,

    BreakdownGenerated = 9,

    DescriptionRewritten = 10,

    NoteAdded = 11,

    AttachmentAdded = 12,

    EmbeddingsGenerated = 13,

    CategoriesGenerated = 14
}
