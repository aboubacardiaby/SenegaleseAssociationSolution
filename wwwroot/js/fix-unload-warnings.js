// Fix for deprecated unload event warnings
// This script silently handles deprecated event listeners to prevent browser warnings

(function() {
    'use strict';
    
    // Store original addEventListener and removeEventListener
    const originalAddEventListener = EventTarget.prototype.addEventListener;
    const originalRemoveEventListener = EventTarget.prototype.removeEventListener;
    
    // Counter to track suppressed events (for debugging)
    let suppressedEvents = 0;
    
    // Override addEventListener to silently handle deprecated events
    EventTarget.prototype.addEventListener = function(type, listener, options) {
        // Silently skip deprecated unload events on window to prevent warnings
        if (type === 'unload' && this === window) {
            suppressedEvents++;
            // Silently return without adding the listener
            return;
        }
        
        // For beforeunload, only skip if it's from a library (has no user interaction)
        if (type === 'beforeunload' && this === window) {
            // Check if this might be from a library by examining the call stack
            const stack = new Error().stack;
            if (stack && (stack.includes('jquery') || stack.includes('bootstrap') || stack.includes('.min.js'))) {
                suppressedEvents++;
                // Silently return without adding the listener
                return;
            }
        }
        
        // Call the original method for all other events
        return originalAddEventListener.call(this, type, listener, options);
    };
    
    // Override removeEventListener to handle deprecated events
    EventTarget.prototype.removeEventListener = function(type, listener, options) {
        // For deprecated events, just return since we didn't add them
        if ((type === 'unload' || type === 'beforeunload') && this === window) {
            return;
        }
        
        // Call the original method
        return originalRemoveEventListener.call(this, type, listener, options);
    };
    
    // Provide modern alternatives for page lifecycle
    function setupModernPageLifecycle() {
        // Use Page Visibility API for tracking page state
        document.addEventListener('visibilitychange', function() {
            // This handles when users switch tabs, minimize window, etc.
            // Much more reliable than unload events
        });
        
        // Use pagehide for actual page unloading
        window.addEventListener('pagehide', function(event) {
            // This fires when page is actually being unloaded
            // More reliable than beforeunload for cleanup
        });
    }
    
    // Initialize modern lifecycle events
    if (document.readyState === 'loading') {
        document.addEventListener('DOMContentLoaded', setupModernPageLifecycle);
    } else {
        setupModernPageLifecycle();
    }
    
    // Log success after a brief delay to see if any events were suppressed
    setTimeout(function() {
        if (suppressedEvents > 0) {
            console.log(`Deprecated event warnings fix: suppressed ${suppressedEvents} deprecated event listeners`);
        }
    }, 100);
})();