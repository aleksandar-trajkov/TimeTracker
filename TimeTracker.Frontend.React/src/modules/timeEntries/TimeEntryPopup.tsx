import React, { useState, useEffect } from 'react';
import type { TimeEntry, Category } from '../../apiCalls/timeEntries';
import { formatDate, formatTime } from '../../helpers/dateTime';
import './TimeEntryPopup.css';

interface TimeEntryPopupProps {
    isOpen: boolean;
    onClose: () => void;
    onSave: (timeEntry: Partial<TimeEntry>) => void;
    timeEntry?: TimeEntry | null;
}

interface FormData {
    description: string;
    date: string;
    startTime: string;
    endTime: string;
    categoryName: string;
}

const TimeEntryPopup: React.FC<TimeEntryPopupProps> = ({ isOpen, onClose, onSave, timeEntry = null }) => {
    const [formData, setFormData] = useState<FormData>({
        description: '',
        date: formatDate(new Date()),
        startTime: '',
        endTime: '',
        categoryName: '',
    });

    // Update form data when timeEntry prop changes
    useEffect(() => {
        if (timeEntry) {
            const fromDate = new Date(timeEntry.from);
            const toDate = timeEntry.to ? new Date(timeEntry.to) : null;
            
            setFormData({
                description: timeEntry.description,
                date: formatDate(fromDate),
                startTime: formatTime(timeEntry.from).slice(0, 5), // Convert to HH:MM format
                endTime: toDate ? formatTime(timeEntry.to!).slice(0, 5) : '',
                categoryName: timeEntry.category?.name || '',
            });
        } else {
            // Reset form for new entry
            setFormData({
                description: '',
                date: formatDate(new Date()),
                startTime: '',
                endTime: '',
                categoryName: '',
            });
        }
    }, [timeEntry, isOpen]);

    const handleInputChange = (e: React.ChangeEvent<HTMLInputElement>) => {
        const { name, value } = e.target;
        setFormData(prev => ({
            ...prev,
            [name]: value
        }));
    };

    const handleSubmit = (e: React.FormEvent) => {
        e.preventDefault();
        
        // Combine date and time to create Date objects
        const fromDateTime = new Date(`${formData.date}T${formData.startTime}`);
        const toDateTime = formData.endTime ? new Date(`${formData.date}T${formData.endTime}`) : undefined;
        
        // Create category object if category name is provided
        const category: Category | null = formData.categoryName 
            ? { id: timeEntry?.category?.id || 0, name: formData.categoryName }
            : null;

        const timeEntryData: Partial<TimeEntry> = {
            id: timeEntry?.id,
            description: formData.description,
            from: fromDateTime,
            to: toDateTime,
            category,
        };

        onSave(timeEntryData);
        onClose();
    };

    if (!isOpen) return null;

    return (
        <div className="popup-overlay">
            <div className="popup-container">
                <div className="popup-header">
                    <h3>{timeEntry ? 'Edit Time Entry' : 'Add Time Entry'}</h3>
                    <button className="close-btn" onClick={onClose}>×</button>
                </div>
                
                <form onSubmit={handleSubmit} className="popup-form">
                    <div className="form-group">
                        <label htmlFor="description">Description</label>
                        <input
                            type="text"
                            id="description"
                            name="description"
                            value={formData.description}
                            onChange={handleInputChange}
                            placeholder="Enter task description"
                            required
                        />
                    </div>

                    <div className="form-row">
                        <div className="form-group">
                            <label htmlFor="date">Date</label>
                            <input
                                type="date"
                                id="date"
                                name="date"
                                value={formData.date}
                                onChange={handleInputChange}
                                required
                            />
                        </div>
                        
                        <div className="form-group">
                            <label htmlFor="categoryName">Category</label>
                            <input
                                type="text"
                                id="categoryName"
                                name="categoryName"
                                value={formData.categoryName}
                                onChange={handleInputChange}
                                placeholder="Category name"
                            />
                        </div>
                    </div>

                    <div className="form-row">
                        <div className="form-group">
                            <label htmlFor="startTime">Start Time</label>
                            <input
                                type="time"
                                id="startTime"
                                name="startTime"
                                value={formData.startTime}
                                onChange={handleInputChange}
                                required
                            />
                        </div>
                        
                        <div className="form-group">
                            <label htmlFor="endTime">End Time</label>
                            <input
                                type="time"
                                id="endTime"
                                name="endTime"
                                value={formData.endTime}
                                onChange={handleInputChange}
                            />
                        </div>
                    </div>

                    <div className="popup-actions">
                        <button type="button" className="btn-cancel" onClick={onClose}>
                            Cancel
                        </button>
                        <button type="submit" className="btn-save">
                            {timeEntry ? 'Update' : 'Save'}
                        </button>
                    </div>
                </form>
            </div>
        </div>
    );
};

export default TimeEntryPopup;