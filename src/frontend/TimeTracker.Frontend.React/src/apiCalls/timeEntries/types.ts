export interface TimeEntriesQueryParams {
    from: Date; // ISO date string
    to: Date; // ISO date string
}

export interface TimeEntryMutationData {
    id?: string;
    description: string;
    from: Date;
    to: Date;
    categoryId: string;
}

