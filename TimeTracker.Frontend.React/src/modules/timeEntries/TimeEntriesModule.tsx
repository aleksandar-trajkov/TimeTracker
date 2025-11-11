import React, { useState } from 'react';
import { useTimeEntriesQuery } from '../../apiCalls/timeEntries';
import type { TimeEntriesQueryParams, TimeEntry } from '../../apiCalls/timeEntries';
import { ScheduleList } from '../../components/schedule';
import { DatePicker } from '../../components/date';
import { addDays, getToday, getTomorrow } from '../../helpers/dateTime';
import TimeEntryPopup from './TimeEntryPopup';

const TimeEntriesModule: React.FC = () => {
    const [queryParams, setQueryParams] = useState<TimeEntriesQueryParams>({
        from: getToday(),
        to: getTomorrow(),
    });
    
    const [isPopupOpen, setIsPopupOpen] = useState(false);
    const [selectedTimeEntry, setSelectedTimeEntry] = useState<TimeEntry | null>(null);

    const { 
        data, 
        isLoading, 
        isError, 
        error 
    } = useTimeEntriesQuery(queryParams);

    const setFromDate = (date: Date | null) => {
        if(date === null) return;
        if(queryParams.to && date >= queryParams.to) {
            setQueryParams({ from: date, to: addDays(date, 1) });
            return;
        }
        setQueryParams(prev => ({ ...prev, from: date }));
    }

    const setToDate = (date: Date | null) => {
        if(date === null) return;
        setQueryParams(prev => ({ ...prev, to: date }));
    }

    const handleRowDoubleClick = (timeEntry: TimeEntry) => {
        setSelectedTimeEntry(timeEntry);
        setIsPopupOpen(true);
    };

    const handlePopupClose = () => {
        setIsPopupOpen(false);
        setSelectedTimeEntry(null);
    };

    const handlePopupSave = (timeEntryData: Partial<TimeEntry>) => {
        // TODO: Implement save functionality
        console.log('Saving time entry:', timeEntryData);
        // Here you would typically call an API to save the time entry
    };

    const timeEntries = data || [];

    return (
        <React.Fragment>
        <div className="container-fluid">
            <div className="row">
                <div className="col-12">
                    <h1>Time Entries</h1>
                </div>
            </div>
            <div className="row">
                <div className="col-2">
                    <DatePicker 
                        id="date-picker-from"
                        name="date-picker-from"
                        label="Start Date"
                        value={queryParams.from}
                        onChange={setFromDate} ></DatePicker>
                </div>
                <div className="col-2">
                    <DatePicker 
                        id="date-picker-to"
                        name="date-picker-to"
                        label="End Date"
                        value={queryParams.to}
                        min={addDays(queryParams.from, 1)}
                        onChange={setToDate} ></DatePicker>
                </div>
            </div>
            <div className="row">
                <div className="col-12">
                    <ScheduleList
                        timeEntries={timeEntries}
                        isLoading={isLoading}
                        isError={isError}
                        error={error}
                        onRowDoubleClick={handleRowDoubleClick}
                    />
                </div>
            </div>
        </div>
        <TimeEntryPopup 
            isOpen={isPopupOpen}
            onClose={handlePopupClose}
            onSave={handlePopupSave}
            timeEntry={selectedTimeEntry}
        />
        </React.Fragment>
    );
};

export default TimeEntriesModule;