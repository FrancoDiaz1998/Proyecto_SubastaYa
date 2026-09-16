import React, { useState, useEffect } from 'react';
import { Clock, AlertTriangle, Flame } from 'lucide-react';

interface CountdownTimerProps {
  targetDate: string;
  isScheduled?: boolean;
  isEnded?: boolean;
  compact?: boolean;
  onExpire?: () => void;
}

export const CountdownTimer: React.FC<CountdownTimerProps> = ({
  targetDate,
  isScheduled = false,
  isEnded = false,
  compact = false,
  onExpire,
}) => {
  const [timeLeft, setTimeLeft] = useState<{
    totalSeconds: number;
    days: number;
    hours: number;
    minutes: number;
    seconds: number;
    hasEnded: boolean;
  }>(() => calculateTimeLeft(targetDate));

  function calculateTimeLeft(target: string) {
    const difference = new Date(target).getTime() - Date.now();
    const totalSeconds = Math.max(0, Math.floor(difference / 1000));

    return {
      totalSeconds,
      days: Math.floor(totalSeconds / (3600 * 24)),
      hours: Math.floor((totalSeconds % (3600 * 24)) / 3600),
      minutes: Math.floor((totalSeconds % 3600) / 60),
      seconds: totalSeconds % 60,
      hasEnded: totalSeconds <= 0,
    };
  }

  useEffect(() => {
    const timer = setInterval(() => {
      const remaining = calculateTimeLeft(targetDate);
      setTimeLeft(remaining);

      if (remaining.hasEnded && onExpire) {
        onExpire();
      }
    }, 1000);

    return () => clearInterval(timer);
  }, [targetDate, onExpire]);

  if (isEnded || timeLeft.hasEnded) {
    return (
      <div
        className={`inline-flex items-center gap-1.5 px-2.5 py-1 rounded-full text-xs font-semibold bg-slate-100 text-slate-600 border border-slate-200 ${
          compact ? 'text-[11px]' : ''
        }`}
      >
        <Clock className="w-3.5 h-3.5 text-slate-400" />
        <span>Subasta Finalizada</span>
      </div>
    );
  }

  if (isScheduled) {
    return (
      <div
        className={`inline-flex items-center gap-1.5 px-2.5 py-1 rounded-full text-xs font-semibold bg-blue-50 text-blue-700 border border-blue-200 ${
          compact ? 'text-[11px]' : ''
        }`}
      >
        <Clock className="w-3.5 h-3.5 text-blue-500" />
        <span>
          Inicia en: {timeLeft.days > 0 ? `${timeLeft.days}d ` : ''}
          {String(timeLeft.hours).padStart(2, '0')}:
          {String(timeLeft.minutes).padStart(2, '0')}:
          {String(timeLeft.seconds).padStart(2, '0')}
        </span>
      </div>
    );
  }

  // Zona Crítica: Menos de 60 segundos (Regla Anti-Sniping)
  const isCritical = timeLeft.totalSeconds <= 60;
  // Advertencia: Menos de 2 minutos
  const isWarning = timeLeft.totalSeconds <= 120 && !isCritical;

  const timerClass = isCritical
    ? 'timer-critical'
    : isWarning
    ? 'timer-warning'
    : 'bg-slate-900 text-white border border-slate-800';

  const formatDisplay = () => {
    if (timeLeft.days > 0) {
      return `${timeLeft.days}d ${timeLeft.hours}h ${timeLeft.minutes}m`;
    }
    if (timeLeft.hours > 0) {
      return `${String(timeLeft.hours).padStart(2, '0')}h ${String(
        timeLeft.minutes
      ).padStart(2, '0')}m ${String(timeLeft.seconds).padStart(2, '0')}s`;
    }
    return `${String(timeLeft.minutes).padStart(2, '0')}:${String(
      timeLeft.seconds
    ).padStart(2, '0')}`;
  };

  return (
    <div
      className={`timer-badge inline-flex items-center gap-1.5 px-2.5 py-1 rounded-lg font-mono text-xs font-bold tracking-wide shadow-xs ${timerClass} ${
        compact ? 'text-[11px] px-2 py-0.5' : ''
      }`}
      title={isCritical ? '¡Zona crítica! Toda puja extenderá el tiempo +2 minutos' : undefined}
    >
      {isCritical ? (
        <Flame className="w-3.5 h-3.5 text-red-500 animate-bounce" />
      ) : isWarning ? (
        <AlertTriangle className="w-3.5 h-3.5 text-amber-500" />
      ) : (
        <Clock className="w-3.5 h-3.5 text-slate-400" />
      )}

      <span>{formatDisplay()}</span>

      {isCritical && (
        <span className="hidden sm:inline-block text-[10px] uppercase tracking-wider font-extrabold text-red-700 bg-red-100 px-1 rounded">
          Crítico
        </span>
      )}
    </div>
  );
};

