import { DateTime } from "luxon";

const dateTimeFormat = 'yyyy-MM-dd\'T\'HH:mm:ss\'Z\'';
const dateFormat = 'yyyy-MM-dd';
const timeFormat = 'HH:mm';

const formatDateTime = (date: Date): string => {
    return DateTime.fromJSDate(date).setZone('UTC').toFormat(dateTimeFormat);
};
const formatTime = (date: Date): string => {
    return DateTime.fromJSDate(date).setZone('UTC').toFormat(timeFormat);
};
const formatDate = (date: Date): string => {
    return DateTime.fromJSDate(date).setZone('UTC').toFormat(dateFormat);
}
const getToday = (): Date => {
    return DateTime.now().setZone('UTC').startOf('day').toJSDate();
}
const getTomorrow = (): Date => {
    return DateTime.now().setZone('UTC').plus({ days: 1 }).startOf('day').toJSDate();
}
const getStartOfDay = (date: Date): Date => {
    return DateTime.fromJSDate(date).setZone('UTC').startOf('day').toJSDate();
}
const getStartOfWeek = (date: Date): Date => {
    return DateTime.fromJSDate(date).setZone('UTC').startOf('week').toJSDate();
}
const getEndOfWeek = (date: Date): Date => {
    return DateTime.fromJSDate(date).setZone('UTC').endOf('week').toJSDate();
}
const addDays = (date: Date, days: number): Date => {
    return DateTime.fromJSDate(date).setZone('UTC').plus({ days }).toJSDate();
}

/// Fixes date strings in the response data to be Date objects

const isIsoDateOnlyString = (value: unknown): value is string => {
    const isoDateFormat = /^\d{4}-\d{2}-\d{2}$/;
    return typeof value === "string" && isoDateFormat.test(value);
}
const isIsoDateString = (value: unknown): value is string => {
    const isoDateFormat = /^\d{4}-\d{2}-\d{2}T\d{2}:\d{2}:\d{2}(?:\.\d*)?(?:[-+]\d{2}:?\d{2}|Z)?$/;
    return typeof value === "string" && isoDateFormat.test(value);
}
const fixDateTimeForResponse = <T>(data: T): T => {

    if (data === null || data === undefined || typeof data !== "object")
        return data;

    const dataObject = data as Record<string, unknown>;
    for (const key of Object.keys(dataObject)) {
        const value = dataObject[key];
        if (isIsoDateString(value)) {
            dataObject[key] = new Date(value);
        }
        else if (typeof value === "object" && value !== null) {
            fixDateTimeForResponse(value);
        }
    }
    return data;
}

export {
    dateTimeFormat,
    dateFormat,
    timeFormat,
    formatDateTime,
    formatTime,
    formatDate,
    getToday,
    getTomorrow,
    getStartOfDay,
    getStartOfWeek,
    getEndOfWeek,
    addDays,
    isIsoDateString,
    isIsoDateOnlyString,
    fixDateTimeForResponse
};