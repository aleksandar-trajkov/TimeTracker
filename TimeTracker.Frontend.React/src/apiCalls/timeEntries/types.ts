export interface TimeEntry {
    id: number;
    description: string;
    from: Date; // ISO date string
    to?: Date; // ISO date string, optional for ongoing entries
    category: Category | null;
}

export interface Category {
    id: number;
    name: string;
}

export interface TimeEntriesQueryParams {
    from: Date; // ISO date string
    to: Date; // ISO date string
}