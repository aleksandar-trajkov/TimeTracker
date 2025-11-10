import React, { useState } from 'react';
import { useTimeEntriesQuery } from '../../apiCalls/timeEntries';
import type { TimeEntriesQueryParams } from '../../apiCalls/timeEntries';
import { ScheduleList } from '../../components/schedule';
import { DatePicker } from '../../components/date';
import { addDays, getToday, getTomorrow } from '../../helpers/dateTime';

const TimeEntriesModule: React.FC = () => {
    const [queryParams, setQueryParams] = useState<TimeEntriesQueryParams>({
        from: getToday(),
        to: getTomorrow(),
    });

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

    const timeEntries = data || [];

    return (
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
                        onChange={(value) => setQueryParams({ ...queryParams, to: value })} ></DatePicker>
                </div>
            </div>
            <div className="row">
                <div className="col-12">
                    <ScheduleList
                        timeEntries={timeEntries}
                        isLoading={isLoading}
                        isError={isError}
                        error={error}
                    />
                </div>
            </div>
        </div>
    );
};

export default TimeEntriesModule;