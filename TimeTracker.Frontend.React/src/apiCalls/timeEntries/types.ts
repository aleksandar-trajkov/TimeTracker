import type { Category } from '../categories';

export interface TimeEntry {
    id: string;
    description: string;
    from: Date; // ISO date string
    to: Date; // ISO date string
    category: Category | null;
}

export interface TimeEntriesQueryParams {
    from: Date; // ISO date string
    to: Date; // ISO date string
}

export const createBlankTimeEntry = (): TimeEntry => ({
    id: '',
    description: '',
    from: new Date(),
    to: new Date(),
    category: null
});