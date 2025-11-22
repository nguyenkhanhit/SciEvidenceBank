// Requires jQuery bundle loaded before this script (that's already in _Layout)
(function () {
    if (window.evidenceActionsInitialized) return;
    window.evidenceActionsInitialized = true;

    var generateUrl = '/Citations/Generate';
    var createUrl = '/Citations/Create';
    var likeUrl = '/Evidence/Like';
    var bookmarkUrl = '/Evidence/Bookmark';

    // Delegated handlers so it works on Index and Details
    $(document).on('click', '.btn-cite', function (e) {
        e.preventDefault();
        var id = $(this).data('id');
        // If modal exists, open it. Otherwise optionally create minimal modal in-place.
        var modal = $('#citation-modal-' + id);
        if (modal.length) {
            modal.css('display', 'flex');
            return;
        }

        // As a fallback, create a simple modal and append to body (keeps behavior consistent)
        var $m = $(
            '<div id="citation-modal-' + id + '" class="citation-modal" style="display:flex;position:fixed;inset:0;align-items:center;justify-content:center;z-index:1200;">' +
            '<div style="width:640px;background:var(--surface);border-radius:8px;padding:18px;box-shadow:0 10px 30px rgba(0,0,0,0.4);">' +
            '<div style="display:flex;justify-content:space-between;align-items:center;">' +
            '<h3 style="margin:0;">Tạo Trích dẫn</h3>' +
            '<button class="citation-close" data-id="' + id + '" style="background:transparent;border:0;font-size:20px;cursor:pointer;">✕</button>' +
            '</div>' +
            '<div id="citation-abstract-' + id + '" style="margin-top:12px;padding:12px;background:var(--muted-surface);border-radius:6px;color:var(--muted-text);"></div>' +
            '<div style="display:flex;gap:8px;margin-top:12px;align-items:center; width:100%;">' +
            '<select id="citation-style-' + id + '" class="citation-style" style="flex:1;padding:10px;border-radius:6px;border:1px solid rgba(0,0,0,0.08);">' +
            '<option value="APA">Kiểu APA</option>' +
            '<option value="MLA">MLA</option>' +
            '<option value="Harvard">Harvard</option>' +
            '<option value="Chicago">Chicago / Turabian</option>' +
            '<option value="IEEE">IEEE</option>' +
            '</select>' +
            '<button class="btn-primary-theme citation-generate" data-id="' + id + '">Tạo</button>' +
            '</div>' +
            '<div style="margin-top:12px;padding:12px;background:var(--muted-surface);border-radius:6px;color:var(--muted-text); width:100%;">' +
            '<textarea id="citation-result-' + id + '" class="citation-result full-width-textarea" style="display:block;width:100%;box-sizing:border-box;height:120px;margin-top:12px;padding:12px;border-radius:6px;border:1px solid rgba(0,0,0,0.08);color:var(--text);" placeholder="Trích dẫn của bạn sẽ xuất hiện ở đây."></textarea>' +
            '</div>' +
            '<div style="display:flex;justify-content:flex-end;gap:8px;margin-top:10px;">' +
            '<button class="btn-outline-theme citation-close" data-id="' + id + '">Đóng</button>' +
            '<button class="btn-primary-theme citation-save" data-id="' + id + '">Sao chép</button>' +
            '</div>' +
            '</div>' +
            '</div>'
        );
        // Attempt to populate abstract text from an element on page if available
        var abstractText = $('.evidence-card[data-evidence-id="' + id + '"]').find('.evidence-abstract').text() || '';
        $m.find('#citation-abstract-' + id).text(abstractText);
        $('body').append($m);
    });

    $(document).on('click', '.citation-close', function (e) {
        e.preventDefault();
        var id = $(this).data('id');
        $('#citation-modal-' + id).hide();
    });

    $(document).on('click', '.citation-generate', function (e) {
        e.preventDefault();
        var id = $(this).data('id');
        var style = $('#citation-style-' + id).val();
        $.getJSON(generateUrl, { id: id, style: style }, function (res) {
            if (res && res.success) {
                var text = res.citation;
                $('#citation-result-' + id).val(text);
                copyToClipboard(text);
            } else {
                alert('Không tạo được trích dẫn.');
            }
        }).fail(function () {
            alert('Lỗi mạng khi tạo trích dẫn.');
        });
    });

    $(document).on('click', '.citation-save', function (e) {
        e.preventDefault();
        var id = $(this).data('id');
        var style = $('#citation-style-' + id).val();
        var text = $('#citation-result-' + id).val();
        if (!text) {
            alert('Vui lòng tạo trích dẫn trước.');
            return;
        }

        copyToClipboard(text);

        $.post(createUrl, { evidenceId: id, style: style, citationText: text }, function (res) {
            if (res && res.success) {
                alert('Đã lưu và sao chép trích dẫn.');
                $('#citation-modal-' + id).hide();

                // Update any citation count UI target
                var card = $('div.evidence-card[data-evidence-id="' + id + '"]');
                if (card.length) {
                    card.find('.meta-citations').text('🔁 ' + (res.citations || '0'));
                }
                // If there's a global sidebar element with data-evidence-id, update it too
                $('.meta-citations[data-evidence-id="' + id + '"]').text('🔁 ' + (res.citations || '0'));
            } else {
                alert('Lưu thất bại. Vui lòng đăng nhập.');
            }
        }).fail(function () {
            alert('Lỗi mạng khi lưu trích dẫn.');
        });
    });

    $(document).on('click', '.btn-like', function (e) {
        e.preventDefault();
        var btn = $(this);
        var id = btn.data('id');
        $.post(likeUrl, { id: id }, function (res) {
            if (res && res.success) {
                var card = $('div.evidence-card[data-evidence-id="' + id + '"]');
                card.find('.meta-likes').text('👍 ' + res.likes);
            }
        }).fail(function () {
            console.warn('Like failed');
        });
    });

    $(document).on('click', '.btn-bookmark', function (e) {
        e.preventDefault();
        var btn = $(this);
        var id = btn.data('id');
        $.post(bookmarkUrl, { id: id }, function (res) {
            if (res && res.success) {
                var card = $('div.evidence-card[data-evidence-id="' + id + '"]');
                if (res.bookmarks !== undefined) {
                    card.find('.meta-bookmarks').text('🔖 ' + res.bookmarks);
                }
                alert('Đã thêm trích dẫn vào thư mục của bạn (demo counter).');
            }
        }).fail(function () {
            console.warn('Bookmark failed');
        });
    });

    function copyToClipboard(text) {
        if (!text) return;
        if (navigator.clipboard && navigator.clipboard.writeText) {
            navigator.clipboard.writeText(text).catch(function () {
                fallbackCopy(text);
            });
        } else {
            fallbackCopy(text);
        }
    }
    function fallbackCopy(text) {
        var ta = document.createElement('textarea');
        ta.value = text;
        ta.style.position = 'fixed';
        ta.style.left = '-9999px';
        document.body.appendChild(ta);
        ta.select();
        try {
            document.execCommand('copy');
        } catch (e) { }
        document.body.removeChild(ta);
    }
})();