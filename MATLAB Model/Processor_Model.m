% Complex CPU Model with Pipeline Implementation
clc; clear;

% Constants
MEMORY_SIZE = 32;    % Expanded memory size for complexity
CACHE_SIZE = 8;      % Cache for faster memory access
CPU_CLOCK_SPEED = 3.50e9; % 3.50 GHz
MEMORY_SPEED = 2.10e9;   % 2.10 GHz
BUS_SPEED = 1.00e9;      % 1.00 GHz

% Initialize Memory and Cache
memory = zeros(1, MEMORY_SIZE);
cache = zeros(1, CACHE_SIZE);
cache_valid = false(1, CACHE_SIZE); % Cache validity flags
cache_hits = 0;
cache_misses = 0;

% Sample Instructions
memory(1) = 6001;  
memory(2) = 1001;  
memory(3) = 2002;  
memory(4) = 3003;  
memory(5) = 7003;  
memory(6) = 4000;  
memory(7) = 5005;

% Registers
PC = 1;   % Program Counter
AC = 0;   % Accumulator
IR = 0;   % Instruction Register

% Control Flags
HALT_FLAG = false;
TOTAL_INSTRUCTIONS = 0;
EXECUTION_TIME = 0;

% Instruction Set
LOAD  = 1;
ADD   = 2;
STORE = 3;
SUB   = 5;
READ  = 6;
WRITE = 7;
HALT  = 4;

% Pipeline Registers
fetch_buffer = 0;
decode_buffer = 0;

% Simulation Loop
while ~HALT_FLAG
    % Pipeline: Fetch Phase
    if PC <= MEMORY_SIZE
        fetch_buffer = memory(PC);
    end
    
    % Pipeline: Decode Phase
    if fetch_buffer > 0
        decode_buffer = fetch_buffer;
        IR = decode_buffer;
        opcode = floor(IR / 1000);
        address = mod(IR, 1000);
    else
        opcode = 0;
    end

    % Execute Phase
    start_time = tic; % Start timing
    switch opcode
        case LOAD
            [AC, cache_hits, cache_misses] = load_from_memory(address, memory, cache, cache_valid);
            fprintf('LOAD from address %d: AC = %d\n', address, AC);
        case ADD
            AC = AC + memory(address);
            fprintf('ADD from address %d: AC = %d\n', address, AC);
        case STORE
            memory(address) = AC;
            fprintf('STORE to address %d: memory[%d] = %d\n', address, address, AC);
        case SUB
            AC = AC - memory(address);
            fprintf('SUB from address %d: AC = %d\n', address, AC);
        case READ
            input_buffer = input('Enter input value: ');
            if input_buffer >= 0 && input_buffer <= 999
                memory(address) = input_buffer;
                fprintf('READ to address %d: input = %d\n', address, input_buffer);
            else
                fprintf('Error: Invalid input!\n');
            end
        case WRITE
            output_buffer = memory(address);
            fprintf('WRITE from address %d: output = %d\n', address, output_buffer);
        case HALT
            HALT_FLAG = true;
            fprintf('HALT encountered. Stopping execution.\n');
        otherwise
            fprintf('Unknown instruction!\n');
    end

    % Time Calculation
    instruction_time = toc(start_time);
    EXECUTION_TIME = EXECUTION_TIME + instruction_time;
    TOTAL_INSTRUCTIONS = TOTAL_INSTRUCTIONS + 1;

    % Update Program Counter
    PC = PC + 1;
end

% Final Report
disp('Final Memory State:');
disp(memory);
disp(['Accumulator (AC) = ', num2str(AC)]);
disp(['Program Counter (PC) = ', num2str(PC)]);
disp(['Total Instructions Executed: ', num2str(TOTAL_INSTRUCTIONS)]);
disp(['Total Execution Time: ', num2str(EXECUTION_TIME), ' seconds']);
disp(['Cache Hits: ', num2str(cache_hits)]);
disp(['Cache Misses: ', num2str(cache_misses)]);

% Cache Function for Loading
function [value, hits, misses] = load_from_memory(address, memory, cache, cache_valid)
    hits = 0;
    misses = 0;
    % Simple Cache Replacement Policy (LRU or Random can be implemented)
    cache_index = mod(address, length(cache)) + 1;

    if cache_valid(cache_index) && cache(cache_index) == memory(address)
        hits = 1; % Cache hit
        value = cache(cache_index);
    else
        misses = 1; % Cache miss
        cache(cache_index) = memory(address); % Load into cache
        cache_valid(cache_index) = true;
        value = memory(address);
    end
end
