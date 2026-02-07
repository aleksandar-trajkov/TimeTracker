import type { Category } from "./Category";

export interface TimeEntry {
    id: string;
    description: string;
    from: Date; // ISO date string
    to: Date; // ISO date string
    category: Category | null;
}

export const createBlankTimeEntry = (): TimeEntry => ({
    id: '',
    description: '',
    from: new Date(),
    to: new Date(),
    category: null
});