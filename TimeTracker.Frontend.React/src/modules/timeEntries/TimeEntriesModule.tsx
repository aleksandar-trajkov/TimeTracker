import React, { useState } from 'react';
import { useTimeEntriesQuery } from '../../apiCalls/timeEntries';
import type { TimeEntriesQueryParams } from '../../apiCalls/timeEntries';

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

    if (isLoading) {
        return (
            <div className="d-flex justify-content-center">
                <div className="spinner-border" role="status">
                    <span className="visually-hidden">Loading...</span>
                </div>
            </div>
        );
    }

    if (isError) {
        return (
            <div className="alert alert-danger" role="alert">
                <h4 className="alert-heading">Error loading time entries</h4>
                <p>{error?.message || 'Failed to load time entries. Please try again.'}</p>
            </div>
        );
    }

    const timeEntries  = data || [];

    const formatTime = (dateTimeString: string): string => {
        return new Date(dateTimeString).toLocaleTimeString('en-US', {
            hour: '2-digit',
            minute: '2-digit'
        });
    };

    const formatDate = (dateString: string): string => {
        return new Date(dateString).toLocaleDateString('en-US', {
            weekday: 'short',
            year: 'numeric',
            month: 'short',
            day: 'numeric'
        });
    };

    return (
        <div className="container-fluid">
            <div className="row">
                <div className="col-12">
                    <div className="d-flex justify-content-between align-items-center mb-4">
                                <h1>Time Entries</h1>
                    </div>

                    {timeEntries.length === 0 ? (
                        <div className="text-center py-5">
                            <h3 className="text-muted">No time entries found</h3>
                            <p className="text-muted">Start tracking your time to see entries here.</p>
                        </div>
                    ) : (
                        <div className="table-responsive">
                            <table className="table table-striped table-hover">
                                <thead className="table-dark">
                                    <tr>
                                        <th>Description</th>
                                        <th>Start Time</th>
                                        <th>End Time</th>
                                        <th>Category</th>
                                    </tr>
                                </thead>
                                <tbody>
                                    {timeEntries.map((entry) => (
                                        <tr key={entry.id}>
                                            <td>{entry.description}</td>
                                            <td>{formatTime(entry.from)}</td>
                                            <td>{entry.to ? formatTime(entry.to) : 'Ongoing'}</td>
                                            <td>
                                                {entry.category && (
                                                    <span className="badge bg-primary">
                                                        {entry.category.name}
                                                    </span>
                                                )}
                                            </td>
                                        </tr>
                                    ))}
                                </tbody>
                            </table>
                        </div>
                    )}
                </div>
            </div>
        </div>
    );
};

export default TimeEntriesModule;