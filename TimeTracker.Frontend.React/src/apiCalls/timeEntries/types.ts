export interface TimeEntry {
    id: number;
    description: string;
    from: string; // ISO date string
    to?: string; // ISO date string, optional for ongoing entries
    category: Category | null;
}

export interface Category {
    id: number;
    name: string;
}

export interface TimeEntriesQueryParams {
    from?: string; // ISO date string
    to?: string; // ISO date string
}