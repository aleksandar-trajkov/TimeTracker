import React, { useState } from 'react';
import { useTimeEntriesQuery } from '../../apiCalls/timeEntries';
import type { TimeEntriesQueryParams } from '../../apiCalls/timeEntries';
import { ScheduleList } from '../../components/schedule';

const TimeEntriesModule: React.FC = () => {
    const [queryParams] = useState<TimeEntriesQueryParams>({
        from: '2025-11-01',
        to: '2025-11-02'
    });

    const { 
        data, 
        isLoading, 
        isError, 
        error 
    } = useTimeEntriesQuery(queryParams);

    const timeEntries = data || [];

    return (
        <div className="container-fluid">
            <div className="row">
                <div className="col-12">
                    <div className="d-flex justify-content-between align-items-center mb-4">
                        <h1>Time Entries</h1>
                    </div>

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